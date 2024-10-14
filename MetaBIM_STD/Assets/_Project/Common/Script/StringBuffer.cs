

public class StringBuffer
{


    /* ============  MC Pop-up Messages  ================ */


    public static StringBufferItem Messaege_ContentCopy = new StringBufferItem(
    "Copied to clipboard",
    "已复制到剪贴板");

    public static StringBufferItem Messaege_RequestDenied = new StringBufferItem(
    "Request denied",
    "无效请求");




    /* ============  MC Pop-up Messages  ================ */

    #region Popup-header

    public static StringBufferItem Messaege_Popup_Confirm = new StringBufferItem(
    "Please Confirm",
    "请求确认");

    public static StringBufferItem Messaege_Popup_Information = new StringBufferItem(
    "Information",
    "信息");

    //Page_Signin.cs  114 149
    public static StringBufferItem Messaege_Popup_Warning = new StringBufferItem(
    "Warning",
    "警告");

    //Page_UserProfile.cs  134
    public static StringBufferItem Messaege_Popup_UserProfile_Warning = new StringBufferItem(
    "Warning", 
    "警告");

    //Page_BIMCompare.cs 608 645
    public static StringBufferItem Messaege_Popup_Error = new StringBufferItem(
    "Error",
    "发生错误");

    //MCBanner.cs  214
    public static StringBufferItem Messaege_Popup_Complete = new StringBufferItem(
    "Complete",
    "完成");

    //AppController.cs  219
    public static StringBufferItem Messaege_Popup_Tip = new StringBufferItem("Tip", "提示");

    //MCBanner.cs  192
    public static StringBufferItem Messaege_Popup_Download = new StringBufferItem("Downloading", "下载中");

    //MCBanner.cs 202
    public static StringBufferItem Messaege_Popup_Redirect = new StringBufferItem("Redirect", "重定向");

    //Page_Signin.cs  134
    public static StringBufferItem Messaege_Popup_Sent = new StringBufferItem("Code Sent", "验证码已发送");

    //MCanner.cs  98
    public static StringBufferItem Messaege_Popup_Welcome = new StringBufferItem("Welcome to MetaBIM", "欢迎来到MetaBIM");
    #endregion

    #region Banner
    //MCBanner.cs 98
    public static StringBufferItem Messaege_Banner_SignIn = new StringBufferItem("Please sign in with your account to access the platform.",
        "请登录您的账号");
    //MCBanner.cs 184
    public static StringBufferItem Messaege_Banner_SignOut = new StringBufferItem("You are not signed in", "你并没有登录");

    //MCBanner.cs  202
    public static StringBufferItem Messaege_Banner_OpenNewSite = new StringBufferItem("Open support site in new window.", "在新的窗口打开支持界面");

    //MCBanner.cs  214
    public static StringBufferItem Messaege_Banner_Download = new StringBufferItem("Click anywhere to download your package",
        "点击任意地方下载你的数据包");

    //MCBanner.cs  219
    public static StringBufferItem Messaege_Banner_DownloadError = new StringBufferItem("Your package is not ready", "你的数据包并没有准备好");

    //MCBanner.cs  219
    public static StringBufferItem Messaege_Banner_PluginError = new StringBufferItem("Plugin Error", "插件错误");

    //MCBanner.cs  192
    public static StringBufferItem Messaege_Banner_ParperingPackage = new StringBufferItem("Preparing your plugin package...",
               "正在准备你的插件数据包...");
    #endregion


    /* ============  CAD Viewer  ================ */

    #region CAD Viewer

    public static StringBufferItem Messaege_Popup_CadFileNotLoad = new StringBufferItem(
    "File not avaialbe to import (version or type)",
    "无法加载文件，因为文件不存在或者版本不匹配");



    #endregion

    #region Workspace Project Package
    /* ============  Project Model Manager  ================ */


    public static StringBufferItem ProjectManage_NoProjectSelected = new StringBufferItem(
    "Select a project before upload",
    "请先选择一个项目");


    //Page_Workspace.cs 430
    public static StringBufferItem ProjectManage_Merge_Fail = new StringBufferItem("Merge is not available in the version", "该版本合并不可用");
    //Page_Workspace.cs 435
    public static StringBufferItem ProjectManage_Reject = new StringBufferItem("Request update reject failed", "更新请求拒绝失败");
    //Page_Workspace.cs 461
    public static StringBufferItem ProjectManage_Unload = new StringBufferItem("Unload this model", "卸载该模型");
    //Page_Workspace.cs 515
    public static StringBufferItem ProjectManage_RefreshFail = new StringBufferItem("Refresh failed", "刷新失败");

    //Page_Workspce.cs 523
    public static StringBufferItem ProjectManage_LastUpdate = new StringBufferItem("Last Update: ", "上次更新");

    //Page_Workspace.cs 541
    public static StringBufferItem ProjectManage_EmptyName = new StringBufferItem("Please enter a project name", "请输入一个项目名称");
    //Page_Workspace.cs 552
    public static StringBufferItem ProjectManage_SameName = new StringBufferItem("Project with same name already exists. Please use another name.", "存在同名项目，请使用另一个名字");
    //Page_Workspace.cs 559
    public static StringBufferItem ProjectManage_Character = new StringBufferItem(" characters.", "字.");
    //Page_Workspace.cs 559
    public static StringBufferItem ProjectManage_NameLength = new StringBufferItem("Project name length can not be longer than ", "项目名称长度不能超过");
    //Page_Workspace.cs 566
    public static StringBufferItem ProjectManage_Confirm = new StringBufferItem("Creating new project ", "创建新项目");

    //Page_Workspace.cs 656
    public static StringBufferItem ProjectManage_OpenModel = new StringBufferItem("Opening ", "正在打开 ");
    //Page_Workspace.cs 656
    public static StringBufferItem ProjectManage_OpenModel_Model = new StringBufferItem(" model(s)", " 模型");
    //Page_Workspace.cs 583 604
    public static StringBufferItem ProjectManage_CreateModel = new StringBufferItem("Add project failed", "添加模型失败");

    //Page_Workspace,cs 644
    public static StringBufferItem ProjectManage_MoreSelectedModel = new StringBufferItem("One or more selected model is still loading, please wait for it to complete or select to open loaded models.", "多个模型正在加载，请等待它们完成或者打开已加载模型");
    //Page_Workspace.cs 652
    public static StringBufferItem ProjectManage_SelectModel = new StringBufferItem("Please select a loaded model to open", "请选择一个已加载文件打开");
    //Page_Workspace.cs 744 754
    public static StringBufferItem ProjectManage_Upload_IFC = new StringBufferItem("Uploading a new IFC model, size limit 200MB", "上传一个大小限制为200MB的IFC文件");
    //Page_Workspace.cs 744 754
    public static StringBufferItem ProjectManage_Uploading_IFC = new StringBufferItem("Upload IFC", "上传IFC");
    //Page_Workspace.cs 792
    public static StringBufferItem ProjectManage_Upload_Format = new StringBufferItem("Upload model only support IFC and file size less than 50MB", "上传模型只支持IFC并且文件大小要小于50MB");

    //Page_Workspace.cs 805 824 
    public static StringBufferItem ProjectManage_OnUploadModel_Support = new StringBufferItem("Selected version of file [", "选择文件版本[");
    //Page_Workspace.cs 805 824
    public static StringBufferItem ProjectManage_OnUploadModel_UnSupport = new StringBufferItem("], is not supported at the moment.", "]此时并不支持");
    //Page_Workspace.cs 805 824
    public static StringBufferItem ProjectManage_OnUploadModel_ModelUpload = new StringBufferItem("Model Upload", "模型已上传");
    //Page_Workspace.cs 808
    public static StringBufferItem ProjectManage_OnUploadModel_Complete = new StringBufferItem("Model Processing takes up to 10 minutes. Please wait for the model to be processed before you can view it.",
        "模型处理会花费10分钟，请耐心等待。");
    //Page_Workspace.cs 808 812 815 818 821
    public static StringBufferItem ProjectManage_OnUploadModel_UploadComplete = new StringBufferItem("Upload Complete", "上传成功");
    //Page_Workspace.cs 812
    public static StringBufferItem ProjectManage_OnUploadModel_NoModelFile = new StringBufferItem("No model file is uploaded", "没有文件被上传");
    //Page_Workspace.cs 815
    public static StringBufferItem ProjectManage_OnUploadModel_OverSize = new StringBufferItem("Model size is too big to upload at the moment (50MB)", "模型过大。无法上传（50MB）");
    //Page_Workspace.cs 818
    public static StringBufferItem ProjectManage_OnUploadModel_Editor = new StringBufferItem("Upload is not functional in editor", "编辑器中上传不可用");
    //Page_Workspace.cs 821
    public static StringBufferItem ProjectManage_OnUploadModel_Multiselect = new StringBufferItem("Please select one file to upload", "请选择一个文件上传");
    #endregion

    #region Page_Project
    //Page_Project.cs  413 425
    public static StringBufferItem PageProject_OnUploadModel_Unsupport1 = new StringBufferItem("Selected version of file [", "选择文件版本[");
    //Page_Project.cs  413 425
    public static StringBufferItem PageProject_OnUploadModel_Unsupport2 = new StringBufferItem("], is not supported at the moment.", "]此时并不支持");
    //Page_Project.cs  413 425
    public static StringBufferItem PageProject_OnUploadModel_ModelUpload = new StringBufferItem("Model Upload", "模型已上传");
    //Page_Project.cs  417
    public static StringBufferItem PageProject_OnUploadModel_Complete = new StringBufferItem("Model Processing takes up to 10 minutes. Please wait for the model to be processed before you can view it.",
        "模型处理会花费10分钟，请耐心等待。");
    //Page_Project.cs  417 420 423
    public static StringBufferItem PageProject_OnUploadModel_UploadComplete = new StringBufferItem("Upload Complete", "上传成功");
    //Page_Project.cs  420
    public static StringBufferItem PageProject_OnUploadModel_NoModelFile = new StringBufferItem("No model file is uploaded", "没有文件被上传");
    //Page_Project.cs  423
    public static StringBufferItem PageProject_OnUploadModel_OverSize = new StringBufferItem("Model size is too big to upload at the moment (50MB)",
        "模型过大。无法上传（50MB）");
    #endregion


    /* ============  BIM Viewer  ================ */
    #region Bim Viewer Buffer

    public static StringBufferItem Classification_Loading_UniclassDocument = new StringBufferItem(
    "Loading Uniclass Table ",
    "加载 Uniclass 数据表 ");

    public static StringBufferItem Classification_Loading_IFCclassDocument = new StringBufferItem(
    "Loading IFC Class ",
    "加载 IFC Class 数据表 ");

    public static StringBufferItem AttributeViewer_Header_Prefix = new StringBufferItem(
    "Properties: ",
    "物件属性: ");

    public static StringBufferItem AttributeViewer_Header_Suffix = new StringBufferItem(
    " element selected",
    "个选中物件  ");

    public static StringBufferItem Zone_AddNewZonePopupHeader = new StringBufferItem(
    "Enter Zone Name",
    "请输入新区域名称");
    //Page_BIMViewer.cs 909
    public static StringBufferItem Zone_InvalidNumber = new StringBufferItem("Invalid number of object selected = ", "无效的物体编号= ");
    //Page_BIMViewer.cs 913
    public static StringBufferItem Zone_SpiltByLevelSection = new StringBufferItem("Split the object by level sections?", "按层级划分物体？");

    //Page_BIMViewer.cs 996
    public static StringBufferItem Zone_SelectAndIsolate = new StringBufferItem("Please select and isolate a target object", "请选择并且分离一个目标物体");
    //Page_BIMViewer.cs 1000
    public static StringBufferItem Zone_SpiltBySectionBox = new StringBufferItem("Split the object by Section Box?", "按区域盒划分？");
    //Page_BIMViewer.cs 1803
    public static StringBufferItem Zone_SpiltBySectionBox_InvalidBCF = new StringBufferItem("Invalid BCF", "无效的BCF");
    //Page_BIMViewer.cs 2002
    public static StringBufferItem Zone_UpdateProjectSucceed = new StringBufferItem("Update Project Succeeded.", "上传文件成功");
    //Page_BIMViewer.cs 2009
    public static StringBufferItem Zone_UpdateProjectFail = new StringBufferItem("Update Project Failed.", "上传文件失败");

    //Page_BIMViewer,cs 2976
    public static StringBufferItem Zone_AddNewZonePopupHeader_Fail = new StringBufferItem("Can not add zone when there are more then one model is activated", "无法添加区域当同时有多个模型激活时");
    //Page_BIMViewer.cs 3048
    public static StringBufferItem Zone_SnapToLastZone_Fail = new StringBufferItem("Plane can not be snapped!", "无法对平面捕捉!");


    public static StringBufferItem Zone_AddNewZoneSameName = new StringBufferItem(
    "Name of a zone already exsits",
    "区域名称已存在");

    public static StringBufferItem Zone_AddNewZonePopupWarning = new StringBufferItem(
    "Name of a zone can not be empty.",
    "新区域名称不能为空");


    public static StringBufferItem Zone_NoZoneItemSelect = new StringBufferItem(
        "No zone is selected",
        "没有选中任何区域物件");


    //Page_BIMViewer.cs 3072
    public static StringBufferItem Zone_ValidateZone = new StringBufferItem("Zoning", "正在分区");
    
    public static StringBufferItem Zone_StartProcessingWarning = new StringBufferItem(
    "Validating zone will take some time to process, do you want to proceed?",
    "区域信息处理需要一些时间，是否继续？");

    public static StringBufferItem Zone_NoZoneFound = new StringBufferItem(
        "No zone has been added",
        "没有区域数据可供处理");

    //Page_BIMViewer.cs 3124
    public static StringBufferItem Zone_OnProcessZoneObject = new StringBufferItem("Setup zone for ", "配置区域给 ");
    //Page_BIMViewer,cs 1216
    public static StringBufferItem Zone_OpenModelLevel = new StringBufferItem("Can not level window when there are more than one model opened.", "无法打开该层级窗口当多个模型被打开");
    //Page_BIMViewer.cs 1175
    public static StringBufferItem Zone_Measurement = new StringBufferItem("Measurement under development", "测量正在开发");
    //Page_BIMViewer.cs 814
    public static StringBufferItem Zone_ProjectSnapshotImage = new StringBufferItem("Model Snapshot Saved", "模型快照保存");

    public static StringBufferItem Zone_CompleteProcessingWarning = new StringBufferItem(
    "Zone processing complete",
    "区域信息处理完成");


    public static StringBufferItem Zone_CompleteApplyValue = new StringBufferItem(
    "Element has been modified to zone: ",
    "模型对象已修改至区域: ");


    public static StringBufferItem Zone_ErrorMutipleZone = new StringBufferItem(
    "Element exsits in mutiple zone",
    "模型对象存在多个区域中");

    public static StringBufferItem Zone_ErrorNoZone = new StringBufferItem(
    "Element not in any zone",
    "模型对象不在区域中");

    public static StringBufferItem Zone_PlaneNotSelected = new StringBufferItem(
    "Please select one plane from new zone.",
    "选择一个平面作为新区域的方向");

    public static StringBufferItem Zone_NotEnoughSpaceForNewZone = new StringBufferItem(
    "Not enough space for new zone",
    "没有足够的空间创建新区域");

    public static StringBufferItem Zone_ObjectNotFound = new StringBufferItem(
    "No Object found for selected attribute.",
    "未找到该属性的对象");



    public static StringBufferItem Zone_SetToShowOriginalModel = new StringBufferItem(
    "Set to show original model",
    "设置为显示原始模型");


    public static StringBufferItem Zone_SetToShowZoneModel = new StringBufferItem(
    "Set to show zone model",
    "设置为显示区域模型");

    public static StringBufferItem Zone_ChangeZoneShowMode = new StringBufferItem(
    "Change zone mode will cancel any hide, isolated and selected object.",
    "改变区域显示模式将会取消所有隐藏、隔离和选中的对象");


    public static StringBufferItem Validation_StartValiatingConfirm = new StringBufferItem(
    "Data validation will take some time to process, do you want to proceed?",
    "属性检测处理需要一些时间，是否继续？");

    public static StringBufferItem Validation_CompleteProcessingWarning = new StringBufferItem(
    "Validatiion processing complete",
    "模型检测处理完成。");


    #endregion

    #region Bim Compare

    //Page_BIMCompare.cs 247
    public static StringBufferItem BIM_Compare_VersionCompare = new StringBufferItem("Process version comparsion", "处理版本比较");
    //Page_BIMCompare.cs 251
    public static StringBufferItem BIM_Compare_NoVersion = new StringBufferItem("No version selected", "没有版本选择");
    //Page_BIMCompare.cs 251
    public static StringBufferItem BIM_Compare_Tip = new StringBufferItem("Version", "版本");

    //Pgae_BIMCompare.cs 261
    public static StringBufferItem BIM_Compare_Comfirm = new StringBufferItem("Version comparson is now in processing, please wait it to complete",
        "版本比较正在进行，请等待它完成");
    //Page_BIMCpmpare.cs 261
    public static StringBufferItem BIM_Compare_ComfirmTip = new StringBufferItem("Processing", "处理中");
    //Page_BIMCompare.cs 608
    public static StringBufferItem BIM_Compare_OnLoadSourceAsset = new StringBufferItem("Source Asset Load Failed", "源资产载入错误");
    //Page_BIMCompare.cs 645
    public static StringBufferItem BIM_Compare_OnLoadTargetAsset = new StringBufferItem("Target Asset Load Failed", "目标资产载入失败");
    #endregion


    /* ============  Share Link  ================ */

    public static StringBufferItem ShareLink_Message_Downloading = new StringBufferItem(
    "Downloading Model...",
    "下载模型...");
    public static StringBufferItem ShareLink_Message_Processing = new StringBufferItem(
    "Processing Model...",
    "处理模型...");

    //Page_Workspace.cs 996
    public static StringBufferItem ShareLink_Message_Created = new StringBufferItem("Share link created", "创建分享链接");



    /* ============  Admin  ================ */
    //Page_BIMViewer.cs 1799
    public static StringBufferItem Admin_Message_OpenProject = new StringBufferItem(
    "Open project ",
    "打开项目 ");
    //Page_Admin.cs 233 239
    public static StringBufferItem Admin_Message_Accept = new StringBufferItem("Accept request ", "接受请求");
    //Page_Admin.cs 233
    public static StringBufferItem Admin_Message_AsServer = new StringBufferItem(" as Admin?", "作为管理员？");
    //Page_Admin.cs 239
    public static StringBufferItem Admin_Message_AsClient = new StringBufferItem(" as Client?", "作为客户端? ");

    /* ================= Clssification Selector ==============*/

    //Page_ClassificationSelector.cs 899
    public static StringBufferItem Classification_Selector_AppleValue = new StringBufferItem("No element selected", "没有物品被选择");

    /* ================= Dashboard ==============*/

    #region Dashboard
    //Page_Dashboard.cs 71
    public static StringBufferItem Dashboard_Hello = new StringBufferItem("Hello ", "你好 ");
    //Page_Dashboard.cs  71
    public static StringBufferItem Dashboard_Hello1 = new StringBufferItem("Welcome to MetaBIM! ", "欢迎来到MetaBIM");
    //Page_Dashboard.cs  71
    public static StringBufferItem Dashboard_Hello2 = new StringBufferItem("The site is currently in beta, we are appreciate your patient and support :)",
        "该网页现在是beta版本，我们非常感谢你的耐心和支持 :)");
    //Page_Dashboard.cs  73
    public static StringBufferItem Dashboard_Platform = new StringBufferItem("Platform:", "平台:");
    //Page_Dashboard.cs  74
    public static StringBufferItem Dashboard_LastIP = new StringBufferItem("Last IP: ", "上一个IP: ");
    //Page_Dashboard.cs  75
    public static StringBufferItem Dashboard_Verion = new StringBufferItem("Version: ", "版本：");
    //Page_Dashboard.cs  145
    public static StringBufferItem Dashboard_CreateWorkspace = new StringBufferItem("Please enter a Project name", "请输入一个项目名称");
    //Page_Dashboard.cs  154
    public static StringBufferItem Dashboard_CreateWorkspace_SameName = new StringBufferItem("Project with same name already exists. Please use another name.",
        "同名项目已存在，请使用另一个名字");
    //Page_Dashboard.cs  161
    public static StringBufferItem Dashboard_NoLonger = new StringBufferItem("Project name length can not be longer than ", "项目名称不能超过 ");
    //Page_Dashboard.cs  161
    public static StringBufferItem Dashboard_Characters = new StringBufferItem(" characters", " 个字符");
    //Page_Dashboard.cs  165
    public static StringBufferItem Dashboard_CreateWorkspace_Confirm = new StringBufferItem("Create new project ", "创建新项目 ");
    //Page_Dashboard.cs  184
    public static StringBufferItem Dashboard_CreateWorkspace_FailTip = new StringBufferItem("Add Project failed", "添加项目失败");
    //Page_Dashboard.cs  198
    public static StringBufferItem Dashboard_CreateWorkspace_CancelTip = new StringBufferItem("New Project Cancelled", "新项目取消");
    //Page_Dashboard.cs  233
    public static StringBufferItem Dashboard_CreateWorkspace_RefreshTip = new StringBufferItem("Refresh Project failed", "刷新项目失败");
    //Page_Dashboard.cs  257
    public static StringBufferItem Dashboard_CreateWorkspace_OpenProject = new StringBufferItem("Open Project  ", "打开项目 ");
    //Page_Dashboard.cs  374
    public static StringBufferItem Dashboard_CreateWorkspace_NetWorkError = new StringBufferItem("Network Error", "网络错误");
    //Page_Dashboard.cs  337
    public static StringBufferItem Dashboard_ExampleProject = new StringBufferItem("An Example Project will be added to your account.",
        "一个示例项目会添加到你的账号中");
    #endregion

    #region Page_Project
    //Page_Project.cs  89
    public static StringBufferItem Project_Message_RefreshFail = new StringBufferItem("Refresh failed", "刷新失败");
    //Page_Project.cs  146
    public static StringBufferItem Project_Message_OperationFail = new StringBufferItem("Operation can not be completed, please try again later."
        , "操作无法完成，请之后重试");
    //Page_Project.cs  150
    public static StringBufferItem Project_Messafe_ProjectUpdate = new StringBufferItem("Project updated.", "项目升级");
    //Page_Project.cs  172
    public static StringBufferItem Project_Message_SelectModel = new StringBufferItem("Please select a model", "请选择一个模型");
    //Page_Project.cs  181
    public static StringBufferItem Project_Messgae_ModelProcessing = new StringBufferItem("Model is still processing.", "模型正在处理．");
    //Page_Project.cs  189
    public static StringBufferItem Project_Message_Loading = new StringBufferItem("Loading ", "载入 ");
    //Page_Project.cs  189
    public static StringBufferItem Project_Message_Model = new StringBufferItem(", may take a while to load", " 需要一点时间加载");
    //Page_Project.cs  256
    public static StringBufferItem Project_Message_Upload = new StringBufferItem("Render member item failed. Message: ", "渲染成员失败, 消息为：");
    //Page_Project.cs  275
    public static StringBufferItem Project_Message_OwnerInvitations = new StringBufferItem("Only owner can send invitations.", "只有拥有者可以发送邀请");
    //Page_Project.cs  293
    public static StringBufferItem Project_Message_InvalidEmail = new StringBufferItem("Invalid email format. Please enter a valid email.",
        "无效的邮箱格式，请输入有效的邮箱格式");
    //Page_Project.cs  302
    public static StringBufferItem Project_Message_InvitationSent = new StringBufferItem("Invitation sent.", "邀请已发送");
    //Page_Project.cs  324
    public static StringBufferItem Project_Message_InvitationAccept = new StringBufferItem("Invitation accepted", "邀请接受");
    //Page_Project.cs  329
    public static StringBufferItem Project_Message_InvitationNotAccept = new StringBufferItem("Could not accept invitation", "无法接受邀请");
    //Page_Project.cs  378
    public static StringBufferItem Project_Message_SelectProject = new StringBufferItem("Please select a project.", "请选择一个项目");
    //Page_Project.cs  382
    public static StringBufferItem Project_Message_UploadIFC = new StringBufferItem("Uploading a new IFC model, size limit 50MB",
        "上传一个新的上限为50MB的IFC模型");
    //Page_Project.cs  400
    public static StringBufferItem Project_Message_UploadIFC_Fail = new StringBufferItem("Upload model only support IFC and size less than 50MB",
        "上传模型仅支持IFC格式并且大小要小于50MB");
    #endregion

    #region Page_Signin



    //Page_Signin.cs  114
    public static StringBufferItem Signin_Message_GetCodePrefix = new StringBufferItem(
    "Get Code (",
    "获取验证码 (");
    
    public static StringBufferItem Signin_Message_GetCodeSurfix = new StringBufferItem(
    ")",
    ")");
    
    public static StringBufferItem Signin_Message_GetCode = new StringBufferItem(
    "Get Code",
    "获取验证码");



    //Page_Signin.cs  114
    public static StringBufferItem Signin_Message_InvalidMobile = new StringBufferItem(
    "Invalid mobile number!",
    "无效手机号!");
    
    
    //Page_Signin.cs  134
    public static StringBufferItem Signin_Message_AuthCode = new StringBufferItem(
    "Your AuthCode has been sent to your mobile.",
    "已发送验证码到你的手机上");
    
    //Page_Signin.cs  149
    public static StringBufferItem Signin_Message_ProfileNotFound = new StringBufferItem("Profile not found!", "该文档没找到");
    //Page_Signin.cs 153 161
    public static StringBufferItem Signin_NetWorkError = new StringBufferItem("Network Error", "网络错误");
    //Page_Signin.cs  154 161
    public static StringBufferItem Signin_Message_SendCodeFail = new StringBufferItem("Send Code Fail", "发送代码错误");
    //Page_Signin.cs  179
    public static StringBufferItem Signin_Message_EnterPIN = new StringBufferItem("Enter 6 digits PIN that was sent to your mobile number",
        "输入6为发送到手机上的PIN码");
    //Page_Signin.cs  197
    public static StringBufferItem Signin_Message_EnterPINFail = new StringBufferItem("Please check if you entered correct PIN",
        "请检查PIN是否输入正确");
    //Page_Signin.cs  197
    public static StringBufferItem Signin_Message_SignInFail = new StringBufferItem("Sign in failed",
               "登录失败");
    #endregion

    #region Page_UserProfile
    //Page_UserProfile.cs 83
    public static StringBufferItem UserProfile_Message_UploadImageFail = new StringBufferItem("Error occured on uploading picture.", "上传图片失败");
    //Page_UserProfile.cs 83 107 112
    public static StringBufferItem UserProfile_Message_UploadFail = new StringBufferItem("Update Failed", "上传失败");
    //Page_UserProfile.cs  112 172
    public static StringBufferItem UserProfile_NetWorkError = new StringBufferItem("Network Error", "网络错误");
    //Page_UserProfile.cs 134
    public static StringBufferItem UserProfile_Message_FieldRequireComplete = new StringBufferItem("One or more field is required to complete",
        "一个或者多个区域需要完成");
    //Page_UserProfile.cs 159
    public static StringBufferItem UserProfile_Message_UpdateProfile = new StringBufferItem("Your profile information has been updated.",
               "你的文档信息已更新");
    //Page_UserProfile.cs 159
    public static StringBufferItem UserProfile_Message_UpdateProfileTitle = new StringBufferItem("Update Profile", "更新文档");
    //Page_UserProfile.cs 167 172
    public static StringBufferItem UserProfile_Message_UpdateProfileFail = new StringBufferItem("Update Failed", "更新文档失败");
    //Page_UserProfile.cs 181
    public static StringBufferItem UserProfile_Message_Welcome = new StringBufferItem("Hello, new friend, thank you for using ",
                      "你好，感谢您使用 ");
    //Page_UserProfile.cs 182
    public static StringBufferItem UserProfile_Message_FillInformation = new StringBufferItem(", please fill in your basic information.",
                             ", 请填写您的基本信息");
    //Page_UserProfile.cs 182
    public static StringBufferItem UserProfile_Message_RegisterRequirement = new StringBufferItem("Registeration Required",
                                    "注册需求");
    #endregion

    #region ModelVersion
    //ModelVersion.cs  82
    public static StringBufferItem ModelVersion_Message_Processing = new StringBufferItem("Model is processing, please wait it to complete.",
               "模型正在处理中，请等待他完成。");
    //ModelVersion.cs  89
    public static StringBufferItem ModelVersion_Message_Download = new StringBufferItem("Model is downloading, please wait it to complete.",
                      "模型正在下载中，请等待他完成。");
    //ModelVersion.cs  100
    public static StringBufferItem ModelVersion_Message_Downloaded = new StringBufferItem("Model is downloaded, clear it to download again",
        "模型已经下载，请清除它并尝试重新下载");
    //ModelVersion.cs  196
    public static StringBufferItem ModelVersion_Message_LoadFail = new StringBufferItem("Model Loading Fail", "模型载入失败");
    #endregion

    #region ProjectModelHandler
    //ProjectModelHandler.cs  172
    public static StringBufferItem ProjectModelHandler_Message_NoVersion = new StringBufferItem("No version of this model can be loaded",
               "没有该模型版本可以被加载");
    //ProjectModelHandler.cs  191
    public static StringBufferItem ProjectModelHandler_Message_Processing = new StringBufferItem("Model is processing, please wait it to complete.",
           "模型正在处理中，请等待他完成。");
    //ProjectModelHandler.cs  191
    public static StringBufferItem ProjectModelHandler_Message_CannotClearWhileLoading = new StringBufferItem("Can not clear model while it's loading",
                      "不能清除模型当他正在加载中");
    //ProjectModelHandler.cs  208
    public static StringBufferItem ProjectModelHandler_Message_NotLoaded = new StringBufferItem("Model is not loaded", "模型没有被加载");
    //ProjectModelHandler.cs  217
    public static StringBufferItem ProjectModelHandler_Message_CannotClear = new StringBufferItem("This model can not be clear", "模型无法被清除");
    //ProjectModelHandler.cs  231
    public static StringBufferItem ProjectModelHandler_Message_Clear = new StringBufferItem("This Model is cleared", "该模型已经被清除");
    #endregion

    #region Other
    //AppController.cs 265
    public static StringBufferItem Controller_SignOut = new StringBufferItem("You are about to sign out from your account!", "您正在尝试登出你的账号！");
    //AppController.cs 265
    public static StringBufferItem Controller_SignOutTitle = new StringBufferItem("Sign Out", "登出");
    //AppController.cs 643
    public static StringBufferItem Controller_CannotShare = new StringBufferItem("Cannot share the data, please check whether the file is opened or used by other program.",
        "无法共享文件数据, 请查看文件是否已打开或被其他程序使用中!");
    //AppController.cs 714
    public static StringBufferItem Controller_CachedData = new StringBufferItem("All cached data is cleared.", "所有的缓存数据已清除");
    //AppUpdater.cs 37
    public static StringBufferItem AppUpdater_NewVersionTip = new StringBufferItem("New Version", "新版本");
    //UIBlock_Project_ModelVersionItem.cs  63
    public static StringBufferItem UIBlock_Project_Clipboard = new StringBufferItem("Version ID saved to clipboard",
               "版本ID已保存在沾粘板");
    //UIBlock_Project_CollaborationItem.cs  64
    public static StringBufferItem UIBlock_MemberFailed = new StringBufferItem("Load project member failed", "加载项目成员失败");
    #endregion
}


public class StringBufferItem
{
    public string S { get { return GetString(); } }

    public StringBufferItem(string _EN, string _ZH)
    {
        EN = _EN;
        ZH = _ZH;
    }



    string EN;
    string ZH;

    string GetString()
    {
        return GetString(ProjectConfiguration.Instance.DefaultLanguage);
    }

    string GetString(ProjectConfiguration.LocationType _location)
    {
        if (_location == ProjectConfiguration.LocationType.EN)
        {
            return EN;
        }
        else
        if (_location == ProjectConfiguration.LocationType.ZH)
        {
            return ZH;
        }

        return EN;

    }

}




