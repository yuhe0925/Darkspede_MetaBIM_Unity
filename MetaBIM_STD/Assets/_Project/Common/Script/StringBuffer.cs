

public class StringBuffer
{


    /* ============  MC Pop-up Messages  ================ */


    public static StringBufferItem Messaege_ContentCopy = new StringBufferItem(
    "Copied to clipboard",
    "�Ѹ��Ƶ�������");

    public static StringBufferItem Messaege_RequestDenied = new StringBufferItem(
    "Request denied",
    "��Ч����");




    /* ============  MC Pop-up Messages  ================ */

    #region Popup-header

    public static StringBufferItem Messaege_Popup_Confirm = new StringBufferItem(
    "Please Confirm",
    "����ȷ��");

    public static StringBufferItem Messaege_Popup_Information = new StringBufferItem(
    "Information",
    "��Ϣ");

    //Page_Signin.cs  114 149
    public static StringBufferItem Messaege_Popup_Warning = new StringBufferItem(
    "Warning",
    "����");

    //Page_UserProfile.cs  134
    public static StringBufferItem Messaege_Popup_UserProfile_Warning = new StringBufferItem(
    "Warning", 
    "����");

    //Page_BIMCompare.cs 608 645
    public static StringBufferItem Messaege_Popup_Error = new StringBufferItem(
    "Error",
    "��������");

    //MCBanner.cs  214
    public static StringBufferItem Messaege_Popup_Complete = new StringBufferItem(
    "Complete",
    "���");

    //AppController.cs  219
    public static StringBufferItem Messaege_Popup_Tip = new StringBufferItem("Tip", "��ʾ");

    //MCBanner.cs  192
    public static StringBufferItem Messaege_Popup_Download = new StringBufferItem("Downloading", "������");

    //MCBanner.cs 202
    public static StringBufferItem Messaege_Popup_Redirect = new StringBufferItem("Redirect", "�ض���");

    //Page_Signin.cs  134
    public static StringBufferItem Messaege_Popup_Sent = new StringBufferItem("Code Sent", "��֤���ѷ���");

    //MCanner.cs  98
    public static StringBufferItem Messaege_Popup_Welcome = new StringBufferItem("Welcome to MetaBIM", "��ӭ����MetaBIM");
    #endregion

    #region Banner
    //MCBanner.cs 98
    public static StringBufferItem Messaege_Banner_SignIn = new StringBufferItem("Please sign in with your account to access the platform.",
        "���¼�����˺�");
    //MCBanner.cs 184
    public static StringBufferItem Messaege_Banner_SignOut = new StringBufferItem("You are not signed in", "�㲢û�е�¼");

    //MCBanner.cs  202
    public static StringBufferItem Messaege_Banner_OpenNewSite = new StringBufferItem("Open support site in new window.", "���µĴ��ڴ�֧�ֽ���");

    //MCBanner.cs  214
    public static StringBufferItem Messaege_Banner_Download = new StringBufferItem("Click anywhere to download your package",
        "�������ط�����������ݰ�");

    //MCBanner.cs  219
    public static StringBufferItem Messaege_Banner_DownloadError = new StringBufferItem("Your package is not ready", "������ݰ���û��׼����");

    //MCBanner.cs  219
    public static StringBufferItem Messaege_Banner_PluginError = new StringBufferItem("Plugin Error", "�������");

    //MCBanner.cs  192
    public static StringBufferItem Messaege_Banner_ParperingPackage = new StringBufferItem("Preparing your plugin package...",
               "����׼����Ĳ�����ݰ�...");
    #endregion


    /* ============  CAD Viewer  ================ */

    #region CAD Viewer

    public static StringBufferItem Messaege_Popup_CadFileNotLoad = new StringBufferItem(
    "File not avaialbe to import (version or type)",
    "�޷������ļ�����Ϊ�ļ������ڻ��߰汾��ƥ��");



    #endregion

    #region Workspace Project Package
    /* ============  Project Model Manager  ================ */


    public static StringBufferItem ProjectManage_NoProjectSelected = new StringBufferItem(
    "Select a project before upload",
    "����ѡ��һ����Ŀ");


    //Page_Workspace.cs 430
    public static StringBufferItem ProjectManage_Merge_Fail = new StringBufferItem("Merge is not available in the version", "�ð汾�ϲ�������");
    //Page_Workspace.cs 435
    public static StringBufferItem ProjectManage_Reject = new StringBufferItem("Request update reject failed", "��������ܾ�ʧ��");
    //Page_Workspace.cs 461
    public static StringBufferItem ProjectManage_Unload = new StringBufferItem("Unload this model", "ж�ظ�ģ��");
    //Page_Workspace.cs 515
    public static StringBufferItem ProjectManage_RefreshFail = new StringBufferItem("Refresh failed", "ˢ��ʧ��");

    //Page_Workspce.cs 523
    public static StringBufferItem ProjectManage_LastUpdate = new StringBufferItem("Last Update: ", "�ϴθ���");

    //Page_Workspace.cs 541
    public static StringBufferItem ProjectManage_EmptyName = new StringBufferItem("Please enter a project name", "������һ����Ŀ����");
    //Page_Workspace.cs 552
    public static StringBufferItem ProjectManage_SameName = new StringBufferItem("Project with same name already exists. Please use another name.", "����ͬ����Ŀ����ʹ����һ������");
    //Page_Workspace.cs 559
    public static StringBufferItem ProjectManage_Character = new StringBufferItem(" characters.", "��.");
    //Page_Workspace.cs 559
    public static StringBufferItem ProjectManage_NameLength = new StringBufferItem("Project name length can not be longer than ", "��Ŀ���Ƴ��Ȳ��ܳ���");
    //Page_Workspace.cs 566
    public static StringBufferItem ProjectManage_Confirm = new StringBufferItem("Creating new project ", "��������Ŀ");

    //Page_Workspace.cs 656
    public static StringBufferItem ProjectManage_OpenModel = new StringBufferItem("Opening ", "���ڴ� ");
    //Page_Workspace.cs 656
    public static StringBufferItem ProjectManage_OpenModel_Model = new StringBufferItem(" model(s)", " ģ��");
    //Page_Workspace.cs 583 604
    public static StringBufferItem ProjectManage_CreateModel = new StringBufferItem("Add project failed", "���ģ��ʧ��");

    //Page_Workspace,cs 644
    public static StringBufferItem ProjectManage_MoreSelectedModel = new StringBufferItem("One or more selected model is still loading, please wait for it to complete or select to open loaded models.", "���ģ�����ڼ��أ���ȴ�������ɻ��ߴ��Ѽ���ģ��");
    //Page_Workspace.cs 652
    public static StringBufferItem ProjectManage_SelectModel = new StringBufferItem("Please select a loaded model to open", "��ѡ��һ���Ѽ����ļ���");
    //Page_Workspace.cs 744 754
    public static StringBufferItem ProjectManage_Upload_IFC = new StringBufferItem("Uploading a new IFC model, size limit 200MB", "�ϴ�һ����С����Ϊ200MB��IFC�ļ�");
    //Page_Workspace.cs 744 754
    public static StringBufferItem ProjectManage_Uploading_IFC = new StringBufferItem("Upload IFC", "�ϴ�IFC");
    //Page_Workspace.cs 792
    public static StringBufferItem ProjectManage_Upload_Format = new StringBufferItem("Upload model only support IFC and file size less than 50MB", "�ϴ�ģ��ֻ֧��IFC�����ļ���СҪС��50MB");

    //Page_Workspace.cs 805 824 
    public static StringBufferItem ProjectManage_OnUploadModel_Support = new StringBufferItem("Selected version of file [", "ѡ���ļ��汾[");
    //Page_Workspace.cs 805 824
    public static StringBufferItem ProjectManage_OnUploadModel_UnSupport = new StringBufferItem("], is not supported at the moment.", "]��ʱ����֧��");
    //Page_Workspace.cs 805 824
    public static StringBufferItem ProjectManage_OnUploadModel_ModelUpload = new StringBufferItem("Model Upload", "ģ�����ϴ�");
    //Page_Workspace.cs 808
    public static StringBufferItem ProjectManage_OnUploadModel_Complete = new StringBufferItem("Model Processing takes up to 10 minutes. Please wait for the model to be processed before you can view it.",
        "ģ�ʹ���Ứ��10���ӣ������ĵȴ���");
    //Page_Workspace.cs 808 812 815 818 821
    public static StringBufferItem ProjectManage_OnUploadModel_UploadComplete = new StringBufferItem("Upload Complete", "�ϴ��ɹ�");
    //Page_Workspace.cs 812
    public static StringBufferItem ProjectManage_OnUploadModel_NoModelFile = new StringBufferItem("No model file is uploaded", "û���ļ����ϴ�");
    //Page_Workspace.cs 815
    public static StringBufferItem ProjectManage_OnUploadModel_OverSize = new StringBufferItem("Model size is too big to upload at the moment (50MB)", "ģ�͹����޷��ϴ���50MB��");
    //Page_Workspace.cs 818
    public static StringBufferItem ProjectManage_OnUploadModel_Editor = new StringBufferItem("Upload is not functional in editor", "�༭�����ϴ�������");
    //Page_Workspace.cs 821
    public static StringBufferItem ProjectManage_OnUploadModel_Multiselect = new StringBufferItem("Please select one file to upload", "��ѡ��һ���ļ��ϴ�");
    #endregion

    #region Page_Project
    //Page_Project.cs  413 425
    public static StringBufferItem PageProject_OnUploadModel_Unsupport1 = new StringBufferItem("Selected version of file [", "ѡ���ļ��汾[");
    //Page_Project.cs  413 425
    public static StringBufferItem PageProject_OnUploadModel_Unsupport2 = new StringBufferItem("], is not supported at the moment.", "]��ʱ����֧��");
    //Page_Project.cs  413 425
    public static StringBufferItem PageProject_OnUploadModel_ModelUpload = new StringBufferItem("Model Upload", "ģ�����ϴ�");
    //Page_Project.cs  417
    public static StringBufferItem PageProject_OnUploadModel_Complete = new StringBufferItem("Model Processing takes up to 10 minutes. Please wait for the model to be processed before you can view it.",
        "ģ�ʹ���Ứ��10���ӣ������ĵȴ���");
    //Page_Project.cs  417 420 423
    public static StringBufferItem PageProject_OnUploadModel_UploadComplete = new StringBufferItem("Upload Complete", "�ϴ��ɹ�");
    //Page_Project.cs  420
    public static StringBufferItem PageProject_OnUploadModel_NoModelFile = new StringBufferItem("No model file is uploaded", "û���ļ����ϴ�");
    //Page_Project.cs  423
    public static StringBufferItem PageProject_OnUploadModel_OverSize = new StringBufferItem("Model size is too big to upload at the moment (50MB)",
        "ģ�͹����޷��ϴ���50MB��");
    #endregion


    /* ============  BIM Viewer  ================ */
    #region Bim Viewer Buffer

    public static StringBufferItem Classification_Loading_UniclassDocument = new StringBufferItem(
    "Loading Uniclass Table ",
    "���� Uniclass ���ݱ� ");

    public static StringBufferItem Classification_Loading_IFCclassDocument = new StringBufferItem(
    "Loading IFC Class ",
    "���� IFC Class ���ݱ� ");

    public static StringBufferItem AttributeViewer_Header_Prefix = new StringBufferItem(
    "Properties: ",
    "�������: ");

    public static StringBufferItem AttributeViewer_Header_Suffix = new StringBufferItem(
    " element selected",
    "��ѡ�����  ");

    public static StringBufferItem Zone_AddNewZonePopupHeader = new StringBufferItem(
    "Enter Zone Name",
    "����������������");
    //Page_BIMViewer.cs 909
    public static StringBufferItem Zone_InvalidNumber = new StringBufferItem("Invalid number of object selected = ", "��Ч��������= ");
    //Page_BIMViewer.cs 913
    public static StringBufferItem Zone_SpiltByLevelSection = new StringBufferItem("Split the object by level sections?", "���㼶�������壿");

    //Page_BIMViewer.cs 996
    public static StringBufferItem Zone_SelectAndIsolate = new StringBufferItem("Please select and isolate a target object", "��ѡ���ҷ���һ��Ŀ������");
    //Page_BIMViewer.cs 1000
    public static StringBufferItem Zone_SpiltBySectionBox = new StringBufferItem("Split the object by Section Box?", "������л��֣�");
    //Page_BIMViewer.cs 1803
    public static StringBufferItem Zone_SpiltBySectionBox_InvalidBCF = new StringBufferItem("Invalid BCF", "��Ч��BCF");
    //Page_BIMViewer.cs 2002
    public static StringBufferItem Zone_UpdateProjectSucceed = new StringBufferItem("Update Project Succeeded.", "�ϴ��ļ��ɹ�");
    //Page_BIMViewer.cs 2009
    public static StringBufferItem Zone_UpdateProjectFail = new StringBufferItem("Update Project Failed.", "�ϴ��ļ�ʧ��");

    //Page_BIMViewer,cs 2976
    public static StringBufferItem Zone_AddNewZonePopupHeader_Fail = new StringBufferItem("Can not add zone when there are more then one model is activated", "�޷��������ͬʱ�ж��ģ�ͼ���ʱ");
    //Page_BIMViewer.cs 3048
    public static StringBufferItem Zone_SnapToLastZone_Fail = new StringBufferItem("Plane can not be snapped!", "�޷���ƽ�沶׽!");


    public static StringBufferItem Zone_AddNewZoneSameName = new StringBufferItem(
    "Name of a zone already exsits",
    "���������Ѵ���");

    public static StringBufferItem Zone_AddNewZonePopupWarning = new StringBufferItem(
    "Name of a zone can not be empty.",
    "���������Ʋ���Ϊ��");


    public static StringBufferItem Zone_NoZoneItemSelect = new StringBufferItem(
        "No zone is selected",
        "û��ѡ���κ��������");


    //Page_BIMViewer.cs 3072
    public static StringBufferItem Zone_ValidateZone = new StringBufferItem("Zoning", "���ڷ���");
    
    public static StringBufferItem Zone_StartProcessingWarning = new StringBufferItem(
    "Validating zone will take some time to process, do you want to proceed?",
    "������Ϣ������ҪһЩʱ�䣬�Ƿ������");

    public static StringBufferItem Zone_NoZoneFound = new StringBufferItem(
        "No zone has been added",
        "û���������ݿɹ�����");

    //Page_BIMViewer.cs 3124
    public static StringBufferItem Zone_OnProcessZoneObject = new StringBufferItem("Setup zone for ", "��������� ");
    //Page_BIMViewer,cs 1216
    public static StringBufferItem Zone_OpenModelLevel = new StringBufferItem("Can not level window when there are more than one model opened.", "�޷��򿪸ò㼶���ڵ����ģ�ͱ���");
    //Page_BIMViewer.cs 1175
    public static StringBufferItem Zone_Measurement = new StringBufferItem("Measurement under development", "�������ڿ���");
    //Page_BIMViewer.cs 814
    public static StringBufferItem Zone_ProjectSnapshotImage = new StringBufferItem("Model Snapshot Saved", "ģ�Ϳ��ձ���");

    public static StringBufferItem Zone_CompleteProcessingWarning = new StringBufferItem(
    "Zone processing complete",
    "������Ϣ�������");


    public static StringBufferItem Zone_CompleteApplyValue = new StringBufferItem(
    "Element has been modified to zone: ",
    "ģ�Ͷ������޸�������: ");


    public static StringBufferItem Zone_ErrorMutipleZone = new StringBufferItem(
    "Element exsits in mutiple zone",
    "ģ�Ͷ�����ڶ��������");

    public static StringBufferItem Zone_ErrorNoZone = new StringBufferItem(
    "Element not in any zone",
    "ģ�Ͷ�����������");

    public static StringBufferItem Zone_PlaneNotSelected = new StringBufferItem(
    "Please select one plane from new zone.",
    "ѡ��һ��ƽ����Ϊ������ķ���");

    public static StringBufferItem Zone_NotEnoughSpaceForNewZone = new StringBufferItem(
    "Not enough space for new zone",
    "û���㹻�Ŀռ䴴��������");

    public static StringBufferItem Zone_ObjectNotFound = new StringBufferItem(
    "No Object found for selected attribute.",
    "δ�ҵ������ԵĶ���");



    public static StringBufferItem Zone_SetToShowOriginalModel = new StringBufferItem(
    "Set to show original model",
    "����Ϊ��ʾԭʼģ��");


    public static StringBufferItem Zone_SetToShowZoneModel = new StringBufferItem(
    "Set to show zone model",
    "����Ϊ��ʾ����ģ��");

    public static StringBufferItem Zone_ChangeZoneShowMode = new StringBufferItem(
    "Change zone mode will cancel any hide, isolated and selected object.",
    "�ı�������ʾģʽ����ȡ���������ء������ѡ�еĶ���");


    public static StringBufferItem Validation_StartValiatingConfirm = new StringBufferItem(
    "Data validation will take some time to process, do you want to proceed?",
    "���Լ�⴦����ҪһЩʱ�䣬�Ƿ������");

    public static StringBufferItem Validation_CompleteProcessingWarning = new StringBufferItem(
    "Validatiion processing complete",
    "ģ�ͼ�⴦����ɡ�");


    #endregion

    #region Bim Compare

    //Page_BIMCompare.cs 247
    public static StringBufferItem BIM_Compare_VersionCompare = new StringBufferItem("Process version comparsion", "����汾�Ƚ�");
    //Page_BIMCompare.cs 251
    public static StringBufferItem BIM_Compare_NoVersion = new StringBufferItem("No version selected", "û�а汾ѡ��");
    //Page_BIMCompare.cs 251
    public static StringBufferItem BIM_Compare_Tip = new StringBufferItem("Version", "�汾");

    //Pgae_BIMCompare.cs 261
    public static StringBufferItem BIM_Compare_Comfirm = new StringBufferItem("Version comparson is now in processing, please wait it to complete",
        "�汾�Ƚ����ڽ��У���ȴ������");
    //Page_BIMCpmpare.cs 261
    public static StringBufferItem BIM_Compare_ComfirmTip = new StringBufferItem("Processing", "������");
    //Page_BIMCompare.cs 608
    public static StringBufferItem BIM_Compare_OnLoadSourceAsset = new StringBufferItem("Source Asset Load Failed", "Դ�ʲ��������");
    //Page_BIMCompare.cs 645
    public static StringBufferItem BIM_Compare_OnLoadTargetAsset = new StringBufferItem("Target Asset Load Failed", "Ŀ���ʲ�����ʧ��");
    #endregion


    /* ============  Share Link  ================ */

    public static StringBufferItem ShareLink_Message_Downloading = new StringBufferItem(
    "Downloading Model...",
    "����ģ��...");
    public static StringBufferItem ShareLink_Message_Processing = new StringBufferItem(
    "Processing Model...",
    "����ģ��...");

    //Page_Workspace.cs 996
    public static StringBufferItem ShareLink_Message_Created = new StringBufferItem("Share link created", "������������");



    /* ============  Admin  ================ */
    //Page_BIMViewer.cs 1799
    public static StringBufferItem Admin_Message_OpenProject = new StringBufferItem(
    "Open project ",
    "����Ŀ ");
    //Page_Admin.cs 233 239
    public static StringBufferItem Admin_Message_Accept = new StringBufferItem("Accept request ", "��������");
    //Page_Admin.cs 233
    public static StringBufferItem Admin_Message_AsServer = new StringBufferItem(" as Admin?", "��Ϊ����Ա��");
    //Page_Admin.cs 239
    public static StringBufferItem Admin_Message_AsClient = new StringBufferItem(" as Client?", "��Ϊ�ͻ���? ");

    /* ================= Clssification Selector ==============*/

    //Page_ClassificationSelector.cs 899
    public static StringBufferItem Classification_Selector_AppleValue = new StringBufferItem("No element selected", "û����Ʒ��ѡ��");

    /* ================= Dashboard ==============*/

    #region Dashboard
    //Page_Dashboard.cs 71
    public static StringBufferItem Dashboard_Hello = new StringBufferItem("Hello ", "��� ");
    //Page_Dashboard.cs  71
    public static StringBufferItem Dashboard_Hello1 = new StringBufferItem("Welcome to MetaBIM! ", "��ӭ����MetaBIM");
    //Page_Dashboard.cs  71
    public static StringBufferItem Dashboard_Hello2 = new StringBufferItem("The site is currently in beta, we are appreciate your patient and support :)",
        "����ҳ������beta�汾�����Ƿǳ���л������ĺ�֧�� :)");
    //Page_Dashboard.cs  73
    public static StringBufferItem Dashboard_Platform = new StringBufferItem("Platform:", "ƽ̨:");
    //Page_Dashboard.cs  74
    public static StringBufferItem Dashboard_LastIP = new StringBufferItem("Last IP: ", "��һ��IP: ");
    //Page_Dashboard.cs  75
    public static StringBufferItem Dashboard_Verion = new StringBufferItem("Version: ", "�汾��");
    //Page_Dashboard.cs  145
    public static StringBufferItem Dashboard_CreateWorkspace = new StringBufferItem("Please enter a Project name", "������һ����Ŀ����");
    //Page_Dashboard.cs  154
    public static StringBufferItem Dashboard_CreateWorkspace_SameName = new StringBufferItem("Project with same name already exists. Please use another name.",
        "ͬ����Ŀ�Ѵ��ڣ���ʹ����һ������");
    //Page_Dashboard.cs  161
    public static StringBufferItem Dashboard_NoLonger = new StringBufferItem("Project name length can not be longer than ", "��Ŀ���Ʋ��ܳ��� ");
    //Page_Dashboard.cs  161
    public static StringBufferItem Dashboard_Characters = new StringBufferItem(" characters", " ���ַ�");
    //Page_Dashboard.cs  165
    public static StringBufferItem Dashboard_CreateWorkspace_Confirm = new StringBufferItem("Create new project ", "��������Ŀ ");
    //Page_Dashboard.cs  184
    public static StringBufferItem Dashboard_CreateWorkspace_FailTip = new StringBufferItem("Add Project failed", "�����Ŀʧ��");
    //Page_Dashboard.cs  198
    public static StringBufferItem Dashboard_CreateWorkspace_CancelTip = new StringBufferItem("New Project Cancelled", "����Ŀȡ��");
    //Page_Dashboard.cs  233
    public static StringBufferItem Dashboard_CreateWorkspace_RefreshTip = new StringBufferItem("Refresh Project failed", "ˢ����Ŀʧ��");
    //Page_Dashboard.cs  257
    public static StringBufferItem Dashboard_CreateWorkspace_OpenProject = new StringBufferItem("Open Project  ", "����Ŀ ");
    //Page_Dashboard.cs  374
    public static StringBufferItem Dashboard_CreateWorkspace_NetWorkError = new StringBufferItem("Network Error", "�������");
    //Page_Dashboard.cs  337
    public static StringBufferItem Dashboard_ExampleProject = new StringBufferItem("An Example Project will be added to your account.",
        "һ��ʾ����Ŀ����ӵ�����˺���");
    #endregion

    #region Page_Project
    //Page_Project.cs  89
    public static StringBufferItem Project_Message_RefreshFail = new StringBufferItem("Refresh failed", "ˢ��ʧ��");
    //Page_Project.cs  146
    public static StringBufferItem Project_Message_OperationFail = new StringBufferItem("Operation can not be completed, please try again later."
        , "�����޷���ɣ���֮������");
    //Page_Project.cs  150
    public static StringBufferItem Project_Messafe_ProjectUpdate = new StringBufferItem("Project updated.", "��Ŀ����");
    //Page_Project.cs  172
    public static StringBufferItem Project_Message_SelectModel = new StringBufferItem("Please select a model", "��ѡ��һ��ģ��");
    //Page_Project.cs  181
    public static StringBufferItem Project_Messgae_ModelProcessing = new StringBufferItem("Model is still processing.", "ģ�����ڴ���");
    //Page_Project.cs  189
    public static StringBufferItem Project_Message_Loading = new StringBufferItem("Loading ", "���� ");
    //Page_Project.cs  189
    public static StringBufferItem Project_Message_Model = new StringBufferItem(", may take a while to load", " ��Ҫһ��ʱ�����");
    //Page_Project.cs  256
    public static StringBufferItem Project_Message_Upload = new StringBufferItem("Render member item failed. Message: ", "��Ⱦ��Աʧ��, ��ϢΪ��");
    //Page_Project.cs  275
    public static StringBufferItem Project_Message_OwnerInvitations = new StringBufferItem("Only owner can send invitations.", "ֻ��ӵ���߿��Է�������");
    //Page_Project.cs  293
    public static StringBufferItem Project_Message_InvalidEmail = new StringBufferItem("Invalid email format. Please enter a valid email.",
        "��Ч�������ʽ����������Ч�������ʽ");
    //Page_Project.cs  302
    public static StringBufferItem Project_Message_InvitationSent = new StringBufferItem("Invitation sent.", "�����ѷ���");
    //Page_Project.cs  324
    public static StringBufferItem Project_Message_InvitationAccept = new StringBufferItem("Invitation accepted", "�������");
    //Page_Project.cs  329
    public static StringBufferItem Project_Message_InvitationNotAccept = new StringBufferItem("Could not accept invitation", "�޷���������");
    //Page_Project.cs  378
    public static StringBufferItem Project_Message_SelectProject = new StringBufferItem("Please select a project.", "��ѡ��һ����Ŀ");
    //Page_Project.cs  382
    public static StringBufferItem Project_Message_UploadIFC = new StringBufferItem("Uploading a new IFC model, size limit 50MB",
        "�ϴ�һ���µ�����Ϊ50MB��IFCģ��");
    //Page_Project.cs  400
    public static StringBufferItem Project_Message_UploadIFC_Fail = new StringBufferItem("Upload model only support IFC and size less than 50MB",
        "�ϴ�ģ�ͽ�֧��IFC��ʽ���Ҵ�СҪС��50MB");
    #endregion

    #region Page_Signin



    //Page_Signin.cs  114
    public static StringBufferItem Signin_Message_GetCodePrefix = new StringBufferItem(
    "Get Code (",
    "��ȡ��֤�� (");
    
    public static StringBufferItem Signin_Message_GetCodeSurfix = new StringBufferItem(
    ")",
    ")");
    
    public static StringBufferItem Signin_Message_GetCode = new StringBufferItem(
    "Get Code",
    "��ȡ��֤��");



    //Page_Signin.cs  114
    public static StringBufferItem Signin_Message_InvalidMobile = new StringBufferItem(
    "Invalid mobile number!",
    "��Ч�ֻ���!");
    
    
    //Page_Signin.cs  134
    public static StringBufferItem Signin_Message_AuthCode = new StringBufferItem(
    "Your AuthCode has been sent to your mobile.",
    "�ѷ�����֤�뵽����ֻ���");
    
    //Page_Signin.cs  149
    public static StringBufferItem Signin_Message_ProfileNotFound = new StringBufferItem("Profile not found!", "���ĵ�û�ҵ�");
    //Page_Signin.cs 153 161
    public static StringBufferItem Signin_NetWorkError = new StringBufferItem("Network Error", "�������");
    //Page_Signin.cs  154 161
    public static StringBufferItem Signin_Message_SendCodeFail = new StringBufferItem("Send Code Fail", "���ʹ������");
    //Page_Signin.cs  179
    public static StringBufferItem Signin_Message_EnterPIN = new StringBufferItem("Enter 6 digits PIN that was sent to your mobile number",
        "����6Ϊ���͵��ֻ��ϵ�PIN��");
    //Page_Signin.cs  197
    public static StringBufferItem Signin_Message_EnterPINFail = new StringBufferItem("Please check if you entered correct PIN",
        "����PIN�Ƿ�������ȷ");
    //Page_Signin.cs  197
    public static StringBufferItem Signin_Message_SignInFail = new StringBufferItem("Sign in failed",
               "��¼ʧ��");
    #endregion

    #region Page_UserProfile
    //Page_UserProfile.cs 83
    public static StringBufferItem UserProfile_Message_UploadImageFail = new StringBufferItem("Error occured on uploading picture.", "�ϴ�ͼƬʧ��");
    //Page_UserProfile.cs 83 107 112
    public static StringBufferItem UserProfile_Message_UploadFail = new StringBufferItem("Update Failed", "�ϴ�ʧ��");
    //Page_UserProfile.cs  112 172
    public static StringBufferItem UserProfile_NetWorkError = new StringBufferItem("Network Error", "�������");
    //Page_UserProfile.cs 134
    public static StringBufferItem UserProfile_Message_FieldRequireComplete = new StringBufferItem("One or more field is required to complete",
        "һ�����߶��������Ҫ���");
    //Page_UserProfile.cs 159
    public static StringBufferItem UserProfile_Message_UpdateProfile = new StringBufferItem("Your profile information has been updated.",
               "����ĵ���Ϣ�Ѹ���");
    //Page_UserProfile.cs 159
    public static StringBufferItem UserProfile_Message_UpdateProfileTitle = new StringBufferItem("Update Profile", "�����ĵ�");
    //Page_UserProfile.cs 167 172
    public static StringBufferItem UserProfile_Message_UpdateProfileFail = new StringBufferItem("Update Failed", "�����ĵ�ʧ��");
    //Page_UserProfile.cs 181
    public static StringBufferItem UserProfile_Message_Welcome = new StringBufferItem("Hello, new friend, thank you for using ",
                      "��ã���л��ʹ�� ");
    //Page_UserProfile.cs 182
    public static StringBufferItem UserProfile_Message_FillInformation = new StringBufferItem(", please fill in your basic information.",
                             ", ����д���Ļ�����Ϣ");
    //Page_UserProfile.cs 182
    public static StringBufferItem UserProfile_Message_RegisterRequirement = new StringBufferItem("Registeration Required",
                                    "ע������");
    #endregion

    #region ModelVersion
    //ModelVersion.cs  82
    public static StringBufferItem ModelVersion_Message_Processing = new StringBufferItem("Model is processing, please wait it to complete.",
               "ģ�����ڴ����У���ȴ�����ɡ�");
    //ModelVersion.cs  89
    public static StringBufferItem ModelVersion_Message_Download = new StringBufferItem("Model is downloading, please wait it to complete.",
                      "ģ�����������У���ȴ�����ɡ�");
    //ModelVersion.cs  100
    public static StringBufferItem ModelVersion_Message_Downloaded = new StringBufferItem("Model is downloaded, clear it to download again",
        "ģ���Ѿ����أ����������������������");
    //ModelVersion.cs  196
    public static StringBufferItem ModelVersion_Message_LoadFail = new StringBufferItem("Model Loading Fail", "ģ������ʧ��");
    #endregion

    #region ProjectModelHandler
    //ProjectModelHandler.cs  172
    public static StringBufferItem ProjectModelHandler_Message_NoVersion = new StringBufferItem("No version of this model can be loaded",
               "û�и�ģ�Ͱ汾���Ա�����");
    //ProjectModelHandler.cs  191
    public static StringBufferItem ProjectModelHandler_Message_Processing = new StringBufferItem("Model is processing, please wait it to complete.",
           "ģ�����ڴ����У���ȴ�����ɡ�");
    //ProjectModelHandler.cs  191
    public static StringBufferItem ProjectModelHandler_Message_CannotClearWhileLoading = new StringBufferItem("Can not clear model while it's loading",
                      "�������ģ�͵������ڼ�����");
    //ProjectModelHandler.cs  208
    public static StringBufferItem ProjectModelHandler_Message_NotLoaded = new StringBufferItem("Model is not loaded", "ģ��û�б�����");
    //ProjectModelHandler.cs  217
    public static StringBufferItem ProjectModelHandler_Message_CannotClear = new StringBufferItem("This model can not be clear", "ģ���޷������");
    //ProjectModelHandler.cs  231
    public static StringBufferItem ProjectModelHandler_Message_Clear = new StringBufferItem("This Model is cleared", "��ģ���Ѿ������");
    #endregion

    #region Other
    //AppController.cs 265
    public static StringBufferItem Controller_SignOut = new StringBufferItem("You are about to sign out from your account!", "�����ڳ��Եǳ�����˺ţ�");
    //AppController.cs 265
    public static StringBufferItem Controller_SignOutTitle = new StringBufferItem("Sign Out", "�ǳ�");
    //AppController.cs 643
    public static StringBufferItem Controller_CannotShare = new StringBufferItem("Cannot share the data, please check whether the file is opened or used by other program.",
        "�޷������ļ�����, ��鿴�ļ��Ƿ��Ѵ򿪻���������ʹ����!");
    //AppController.cs 714
    public static StringBufferItem Controller_CachedData = new StringBufferItem("All cached data is cleared.", "���еĻ������������");
    //AppUpdater.cs 37
    public static StringBufferItem AppUpdater_NewVersionTip = new StringBufferItem("New Version", "�°汾");
    //UIBlock_Project_ModelVersionItem.cs  63
    public static StringBufferItem UIBlock_Project_Clipboard = new StringBufferItem("Version ID saved to clipboard",
               "�汾ID�ѱ�����մճ��");
    //UIBlock_Project_CollaborationItem.cs  64
    public static StringBufferItem UIBlock_MemberFailed = new StringBufferItem("Load project member failed", "������Ŀ��Աʧ��");
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




