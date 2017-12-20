using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor.Tools
{
    public class ABResDepend
    {

        public static void BuildDepend()
        {
            List<ABSingleRes> list = BuildSingle();
            BuildGroup(list);
        }

        private static List<ABSingleRes> BuildSingle()
        {
            ProgressBarUtil.Show("BuildSingle", "BuildSingle",0);

            Dictionary<string, ABSingleRes> mapping = new Dictionary<string, ABSingleRes>();
            List<ABSingleRes> list = new List<ABSingleRes>();

            string[] abNames = AssetDatabase.GetAllAssetBundleNames();
            for (int i = 0; i < abNames.Length; i++)
            {
                string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(abNames[i]);

                for (int j = 0; j < assetPaths.Length; j++)
                {
                    string assetPath = assetPaths[j];
                    ABSingleRes res = new ABSingleRes(assetPath);
                    list.Add(res);
                    mapping.Add(assetPath,res);
                }
            }

            int index = 0;
            while (index < list.Count)
            {
                ABSingleRes curABRes = list[index];
                index++;

                string[] dependencies = AssetDatabase.GetDependencies(curABRes.path, false);
                for (int i = 0; i < dependencies.Length; i++)
                {
                    string curDepend = dependencies[i];
                    if (curDepend.EndsWith(".cs") || curDepend.EndsWith(".js"))
                    {
                        continue;
                    }
                    ABSingleRes dependABRes = null;
                    if (!mapping.TryGetValue(curDepend,out dependABRes))
                    {
                        dependABRes = new ABSingleRes(curDepend);
                        mapping.Add(curDepend,dependABRes);
                        list.Add(dependABRes);
                    }
                    dependABRes.AddParent(curABRes);
                    curABRes.AddChild(dependABRes);
                }

                ProgressBarUtil.Percent = (float) index/(float) list.Count;
            }

            ProgressBarUtil.Close();
            return list;
        }

        private static void BuildGroup(List<ABSingleRes> list)
        {
            List<ABGroupRes> groupList = new List<ABGroupRes>();
            Dictionary<string,ABGroupRes> abMapping = new Dictionary<string, ABGroupRes>();
            Dictionary<string,ABGroupRes> pathMapping = new Dictionary<string, ABGroupRes>();

            Dictionary<ABResEnum,List<ABSingleRes>> typeList = new Dictionary<ABResEnum, List<ABSingleRes>>(); 

            //带AssetBundleName的资源生成ABGroupRes
            ProgressBarUtil.Show("生成ABGroupRes", "带AssetBundleName资源", 0);
            for (int i = 0; i < list.Count; i++)
            {
                ABSingleRes abSingleRes = list[i];
                if (!string.IsNullOrEmpty(abSingleRes.AssetBundleName))
                {
                    ABGroupRes abGroupRes = null;
                    if (!abMapping.TryGetValue(abSingleRes.AssetBundleName,out abGroupRes))
                    {
                        abGroupRes = new ABGroupRes(abSingleRes.AssetBundleName, abSingleRes.AssetBundleName, true);
                        abGroupRes.AssetBundleName = abSingleRes.AssetBundleName;
                        groupList.Add(abGroupRes);
                        abMapping.Add(abSingleRes.AssetBundleName,abGroupRes);
                    }

                    abGroupRes.Add(abSingleRes);
                    pathMapping.Add(abSingleRes.path,abGroupRes);
                }
                else
                {
                    List<ABSingleRes> reses = null;
                    if (!typeList.TryGetValue(abSingleRes.type,out reses))
                    {
                        reses = new List<ABSingleRes>();
                        typeList.Add(abSingleRes.type,reses);
                    }
                    reses.Add(abSingleRes);
                }

                ProgressBarUtil.Percent = (float)i / (float)list.Count;
            }

            //带AssetBundleName的资源生成ABGroupRes
            foreach (KeyValuePair<ABResEnum, List<ABSingleRes>> pair in typeList)
            {
                if (pair.Key == ABResEnum.atlas)
                {
                    BuildABNameGroup(pair.Value,ref groupList,ref abMapping,ref pathMapping);
                }
                else
                {
                    BuildPathGroup(pair.Value, ref groupList, ref abMapping, ref pathMapping);
                }
            }

            //构造引用关系
            ProgressBarUtil.Show("构造引用关系", "构造引用关系", 0);
            for (int i = 0; i < groupList.Count; i++)
            {
                ABGroupRes res = groupList[i];
                for (int j = 0; j < res.groups.Count; j++)
                {
                    ABRes target = res.groups[j];
                    for (int k = 0; k < target.childs.Count; k++)
                    {
                        string path = target.childs[k].path;
                        ABGroupRes childGroup = pathMapping[path];
                        if (childGroup != res)
                        {
                            res.AddChild(childGroup);
                        }
                    }

                    for (int k = 0; k < target.parents.Count; k++)
                    {
                        string path = target.parents[k].path;
                        ABGroupRes parentGroup = pathMapping[path];
                        if (parentGroup != res)
                        {
                            res.AddParent(parentGroup);
                        }
                    }
                }

                ProgressBarUtil.Percent = (float)i / (float)groupList.Count;
            }

            //清除重复节点  A->B->C  A->C   可将A->C移除
            ProgressBarUtil.Show("裁剪节点", "裁剪重复依赖", 0);
            List<ABRes> stack = new List<ABRes>();
            List<ABRes> parents = new List<ABRes>();
            for (int i = 0; i < groupList.Count; i++)
            {
                ABGroupRes res = groupList[i];
                parents.Clear();
                parents.AddRange(res.parents);

                stack.Clear();
                for (int j = 0; j < parents.Count; j++)
                {
                    if (res.parents.Contains(parents[j]))
                    {
                        RecursionCut(res, parents[j], stack);
                    }
                }

                ProgressBarUtil.Percent = (float)i / (float)groupList.Count;
            }

            //材质不打包，处理依赖问题
            ProgressBarUtil.Show("裁剪节点", "裁剪材质节点", 0);
            List<ABSingleRes> matResList = null;
            if (typeList.TryGetValue(ABResEnum.material, out matResList))
            {
                for (int i = 0; i < matResList.Count; i++)
                {
                    ABSingleRes res = matResList[i];
                    ABGroupRes matGroupRes = pathMapping[res.path];
                    for (int j = 0; j < res.parents.Count; j++)
                    {
                        ABSingleRes parentRes = res.parents[j] as ABSingleRes;
                        if (parentRes.type == ABResEnum.material)
                        {
                            throw new Exception("材质的parent是材质");
                        }

                        ABGroupRes groupRes = pathMapping[parentRes.path];
                        groupRes.RemoveChild(matGroupRes);
                        for (int k = 0; k < res.childs.Count; k++)
                        {
                            ABSingleRes childRes = res.childs[k] as ABSingleRes;
                            if (childRes.type == ABResEnum.material)
                            {
                                throw new Exception("材质的child是材质");
                            }

                            ABGroupRes childGroupRes = pathMapping[childRes.path];
                            groupRes.AddChild(childGroupRes);
                            childGroupRes.AddParent(groupRes);
                        }
                    }
                    for (int k = 0; k < res.childs.Count; k++)
                    {
                        ABSingleRes childRes = res.childs[k] as ABSingleRes;
 
                        ABGroupRes childGroupRes = pathMapping[childRes.path];
                        childGroupRes.RemoveParent(matGroupRes);
                    }
                    matGroupRes.ClearParent();
                    matGroupRes.ClearChild();

                    ProgressBarUtil.Percent = (float)i / (float)matResList.Count;
                }
            }

            //设置AssetBundle
            ProgressBarUtil.Show("设置AssetBundle", "设置AssetBundle",0);
            HashSet<ABRes> set = new HashSet<ABRes>();
            for (int i = 0; i < groupList.Count; i++)
            {
                if (groupList[i].parents.Count > 0)
                {
                    RecursionSetABName(groupList[i], set);
                }
                ProgressBarUtil.Percent = (float)i / (float)groupList.Count;
            }

            ProgressBarUtil.Close();
        }

        private static void RecursionCut(ABRes target, ABRes current, List<ABRes> list)
        {
            if (list.Contains(current) || target == current)
            {
                return;
            }

            if (target.parents.Contains(current) && list.Count > 0 && list[0] != current)
            {
                target.RemoveParent(current);
                current.RemoveChild(target);
                Debug.Log("移除依赖:" + current.path + "->" + target.path);
                return;
            }

            list.Add(current);

            for (int i = 0; i < current.parents.Count; i++)
            {
                RecursionCut(target,current.parents[i],list);
            }

            list.Remove(current);
        }

        private static void RecursionSetABName(ABRes abRes,HashSet<ABRes> set)
        {
            if (set.Contains(abRes) || !String.IsNullOrEmpty(abRes.SaveInAssetBundleName))
            {
                return;
            }

            set.Add(abRes);

            string abName = String.Empty;
            bool package = false;
            for (int i = 0; i < abRes.parents.Count; i++)
            {
                RecursionSetABName(abRes.parents[i],set);

                string parentABName = abRes.parents[i].SaveInAssetBundleName;
                if (String.IsNullOrEmpty(abName))
                {
                    abName = parentABName;
                }
                else if(abName != parentABName)
                {
                    package = true;
                }
            }

            if (package)
            {
                abRes.AssetBundleName = abRes.path;
            }
            else
            {
                abRes.SaveInAssetBundleName = abName;
            }
        }

        private static void BuildPathGroup(List<ABSingleRes> list, ref List<ABGroupRes> groupList, ref Dictionary<string, ABGroupRes> abMapping, ref Dictionary<string, ABGroupRes> pathMapping)
        {
            ProgressBarUtil.Show("生成ABGroupRes", "BuildPathGroup", 0);
            for (int i = 0; i < list.Count; i++)
            {
                ABSingleRes res = list[i];
                string abName = ABResUtil.GetABName(res.path, res.type, res.importer);
                AddGroup(res.path, abName, res,ref groupList,ref abMapping,ref pathMapping);

                ProgressBarUtil.Percent = (float)i / (float)list.Count;
            }

            ProgressBarUtil.Close();
        }

        private static void BuildABNameGroup(List<ABSingleRes> list, ref List<ABGroupRes> groupList, ref Dictionary<string, ABGroupRes> abMapping, ref Dictionary<string, ABGroupRes> pathMapping)
        {
            ProgressBarUtil.Show("生成ABGroupRes", "BuildABNameGroup", 0);
            for (int i = 0; i < list.Count; i++)
            {
                ABSingleRes res = list[i];
                string abName = ABResUtil.GetABName(res.path, res.type, res.importer);
                AddGroup(abName,abName, res, ref groupList, ref abMapping, ref pathMapping);

                ProgressBarUtil.Percent = (float)i / (float)list.Count;
            }

            ProgressBarUtil.Close();
        }

        private static void AddGroup(string key,string path, ABSingleRes res, ref List<ABGroupRes> groupList,ref Dictionary<string, ABGroupRes> abMapping, ref Dictionary<string, ABGroupRes> pathMapping)
        {
            ABGroupRes groupRes = null;
            if (!abMapping.TryGetValue(key, out groupRes))
            {
                groupRes = new ABGroupRes(key,path);
                groupList.Add(groupRes);
                abMapping.Add(key, groupRes);
            }

            groupRes.Add(res);
            pathMapping.Add(res.path,groupRes);
        }
    }
}