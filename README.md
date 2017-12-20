# UnityCooperation

Unity项目美术与开发协作的工作方式

*美术与开发使用不同项目，ProjectDev是开发项目，ProjectArt是美术项目，Assets/Project/Art是公共目录（使用符号链接实现）。这样做的既能保护开发代码，也方便开发项目同步美术资源

*ProjectArt项目 Assets/Project/Art/Editor中提供自动化导入设置功能，包括AssetBundle设置、Model导入设置与Texture导入设置。使用正则表达式方式配置在EditorConfig中

*ProjectDev项目 Assets/Project/Art/Editor中提供发布功能，用于发布Android包与IOS，还实现了热更新包的生成(只是实现生成热更新包的功能，热更新包的部署、下载、解压、使用没有实现)。另外一般发布功能都需要提供开发、测试与线上的版本，这里没有实现，可以自行根据项目需求实现

*AssetBundle的依赖处理实现（AssetBunelTool.BuildDepend），依赖处理需要构造资源的引用关系，耗时比较长，并且会修改资源文件（设置AssetBundle）