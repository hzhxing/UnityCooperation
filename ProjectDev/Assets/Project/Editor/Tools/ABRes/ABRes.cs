using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace Editor.Tools
{
    public class ABRes
    {
        public string path = String.Empty;
        public List<ABRes> parents = new List<ABRes>();
        public List<ABRes> childs = new List<ABRes>();

        public ABRes(string path)
        {
            this.path = path;
        }

        public void AddParent(ABRes res)
        {
            if (parents.Contains(res))
            {
                return;
            }
            parents.Add(res);
        }

        public void RemoveParent(ABRes res)
        {
            parents.Remove(res);
        }

        public void ClearParent()
        {
            parents.Clear();
        }

        public void AddChild(ABRes res)
        {
            if (childs.Contains(res))
            {
                return;
            }
            childs.Add(res);
        }

        public void RemoveChild(ABRes res)
        {
            childs.Remove(res);
        }

        public void ClearChild()
        {
            childs.Clear();
        }

        protected string mSaveInAssetBundleName = String.Empty;
        public string SaveInAssetBundleName
        {
            get { return mSaveInAssetBundleName; }
            set { mSaveInAssetBundleName = value; }
        }

        protected string mAssetBundleName = String.Empty;
        public string AssetBundleName
        {
            get { return mAssetBundleName; }
            set {
                if (mAssetBundleName == value)
                {
                    return;
                }
                mAssetBundleName = value;
                mSaveInAssetBundleName = value;
                OnSetAssetBundle(mAssetBundleName);
            }
        }

        protected virtual void OnSetAssetBundle(string assetBundleName)
        {
            
        }
    }

    public class ABSingleRes : ABRes
    {
        public ABResEnum type = ABResEnum.prefab;
        public AssetImporter importer = null;

        public ABSingleRes(string path) : base(path)
        {
            importer = AssetImporter.GetAtPath(this.path);
            mAssetBundleName = importer.assetBundleName;
            type = ABResUtil.GetType(this.path, importer);
        }

        protected override void OnSetAssetBundle(string assetBundleName)
        {
            importer.assetBundleName = assetBundleName;
        }
    }

    public class ABGroupRes : ABRes
    {
        public List<ABRes> groups = new List<ABRes>(); 

        private bool mRoot = false;
        public bool Root
        {
            get { return mRoot; }
        }

        private string mKey = String.Empty;
        public string Key
        {
            get { return mKey; }
        }

        public ABGroupRes(string key,string path,bool root = false) : base(path)
        {
            mRoot = root;
            mKey = key;
        }

        public void Add(ABRes res)
        {
            if (groups.Contains(res))
            {
                return;
            }
            groups.Add(res);
        }

        public void Remove(ABRes res)
        {
            groups.Remove(res);
        }

        protected override void OnSetAssetBundle(string assetBundleName)
        {
            foreach (ABRes abRes in groups)
            {
                abRes.AssetBundleName = assetBundleName;
            }
        }
    }
}