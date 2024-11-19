# PackManager
1.功能：构建一个简单的本地插件存储管理库。
	全程无实际的文件，删、改、移动 逻辑,需要手动。
2.使用：
	1) 在菜单中找到Tools/PackManagerConfig打开配置窗口.
	2) 初始没有配置文件
		(1) 点击'创建.MPack'文件,在磁盘选择位置任意中一个文件夹,创建配置文件。 其应当是一个空文件夹。
		(2) 选择'读入.MPack'文件,
			会出现两次选择弹窗, 第一次是选择创建的'.MPack'文件,第二次是选择放置'.asset'的位置。
			应当创建为任意Resources下的Tools/Config/PackageManagerConfig.asset。
			规则在PackMgrConfigLoader及其父类中声明。
	3) 无数据的PackManagerConfig窗口
		(1) 点击'新建'按钮
			在打开的窗口中填写数据:
			包名
			目录名 
			说明
			UnionNamespace: 位于最前的命名空间是否是(个人工具标识).
		(2) 创建后对应文件夹被创建
			此时应当弹出了对应目录的资源管理器面板。
		(3) 手动移动数据进去。
	4) 使用PackManagerConfig窗口
		(1) '保存', 关闭窗口也会触发保存,此逻辑写在了对应窗口类.
***			只有发生在'PackManagerConfig窗口'的保存才会发生'存储库配置文件(.MPack)'的修改。
		(2) '依赖完整', 包之间可能存在一些依赖关系,在'详细'->'选择依赖'中设置.
			依赖分为1和2,
			依赖1是在本包管理器中可以访问到的包,
			依赖2是来自其他源的包,
			'依赖完整'只会检测依赖1是否完整。
		(3) UnionNamespace、HasReadme、已安装，按实际情况进行设置.
		(4) '详情'按钮
			可以查看一个包的详细信息;
			可以设置包的git目录路径,方便上传。
		(5) '发布新版'
			点击'详细'按钮, 发布新版功能设置在PackDelta面板。

3.小注
	1) 包内声明了一个程序集'PackManager'.
	2) 从".MPack"文件重新恢复一个".asset"可以复原绝大多数数据。
		已安装无法恢复,考虑到".asset"文件很难丢失,没有写一些预防逻辑。
		这里说明,PackItemBody中的isInstal是private的, 将其置为pulice则可保存, 但其他项目就需要独立的配置并且对库进行的更新不能同步。
	3) 删改建议:
***		备份ConfigAsset然后直接在上面改就行。
***		有任一正确的.MPack或ConfigAsset文件就能恢复。

4.目录标准
	1) 子目录 Type
		如果包应当被放在Scripts目录下时,Type目录实际作为Scripts目录使用。
		存放大多数脚本,只有'Demo相关'和'应当每个项目都要改动的脚本'允许放在外面。
		子目录 Type/Data: 存储数据类
			后缀Asset: ScriptableObject类;
			后缀Loader: 负责'饿汉模式'的加载;
			后缀Body: 某种最小数据存储单元;
		子目录 Type/Editor: 编辑器类
			尽量将类定义为internal的。
***			(1) 如果打入了程序集,那么Editor应当移到不会被打入程序集的位置。
***			(2) 如果包整个不会被发布时使用, 那么Editor仍应当为'Type/Editor', 
				此时设置程序集(AssemblyDefinitionAsset)的'Platforms'页签内只有'Editor'被勾选。
	2) 子目录 Resources
		存放需要通过Resources.Load获取的一些Asset.
		这些Asset不应该直接作为Resources的子文件, 放置在几乎不可能出现意外重复的路径里。
		如Log包的Asset,存放在Resources/Tools/Config.
	3) 子目录 Demo
		任何和Demo/Example强相关的文件都放置在其下.
	4）子文件 Readme.txt
		Readme使用.txt格式,作为局部根目录的直接子文件.

5.尾注
***	1) 为了降低待机性能占用，窗口有很多计算量，绘制蛮消耗性能的, 不用的时候记得关。
	2) 打开资源管理器操作, 不确定在Mac上是否能运行。
		// "explorer.exe" 即资源管理器
		// path是目录路径, 对目录路径格式严格,需要用Path类转换一下 书写不严格(如使用/而非\\) 的path.
		System.Diagnostics.Process.Start("explorer.exe",Path.GetFullPath(path));
		// 如果想正常打开.exe, 传入.exe的路径即可
		System.Diagnostics.Process.Start("H:\\MC\\plain craft launcher 2.exe");
	3) 打开URL: Application.OpenURL(string url);
		// 很多功能被定义在了Editor/PackMgrFunction.cs中。
	4) 语法糖
		(1) var v = array[^1];
			var v = array[array.Lenght-1];
		(2) return t ?? new T();
			return t == null? (t = new T()) : t;
