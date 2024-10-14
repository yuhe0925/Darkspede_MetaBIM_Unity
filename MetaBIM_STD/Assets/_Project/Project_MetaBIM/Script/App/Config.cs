#define DEV // Remove this for production

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace MetaBIM
{
    public class Config
    {
#if (!DEV)
        public static string ProductionName = "MetaBIM";
        public static string SupportSite = "https://metabim.com.au/platform.html";
        public static string Domain = "https://platform.metabim.com.au/";
        public static string DevelopmentStage = "dev";
        public static string API_Key = "j1RpMum08UKS1oT7r7E1bA";
        public static string DatabaseConnectionString = "mongodb://admin:pwd2023!@metabim.com.au:25535/";
        public static string LoggerConnectionString = "mongodb://admin:pwd2023!@metabim.com.au:23333/";

        public static string DatabaseName = "metabim";
        
#else
        public static string ProductionName = "MetaBIM";
        public static string SupportSite = "https://dev.metabim.com.au/platform.html";
        public static string Domain = "https://platformdev.metabim.com.au/";
        public static string Domain_EasyCarbon      = "https://appdev.easycarbon.com.au/";
        public static string DevelopmentStage = "dev";
        public static string API_Key = "j1RpMum08UKS1oT7r7E1bA";
        public static string DatabaseConnectionString = "mongodb://admin:pwd2023!@metabim.com.au:25535/";
        public static string LoggerConnectionString = "mongodb://admin:pwd2023!@metabim.com.au:23333/";

        public static string DatabaseName = "metabim";



#endif

        public static string API_Domain = Domain + "api/client/";
        public static string Resouce_Root = Domain + "resource/";
        public static string UserIcon_Path = Resouce_Root + "usericon/";
        public static string Asset_Path = Resouce_Root + "assetbundle/";
        public static string XML_Path = Resouce_Root + "xml/";
        public static string ProjectImage_Path = Resouce_Root + "project/";
        public static string BCFImage_Path = Resouce_Root + "bcf/";
        public static string ProjectSnaphotImage_Path = Resouce_Root + "project/";
        public static string Package_Path = Resouce_Root + "package/";
        public static string Workspace_Path = Resouce_Root + "workspace/";
        public static string Icon_Default = UserIcon_Path + "default.png";
        public static string Image_Default = UserIcon_Path + "logo.png";


        public static void SetURL(string _updatedDomain)
        {
            API_Domain = _updatedDomain;
            API_Domain = Domain + "api/client/";
            Resouce_Root = Domain + "resource/";
            UserIcon_Path = Resouce_Root + "usericon/";
            Asset_Path = Resouce_Root + "assetbundle/";
            XML_Path = Resouce_Root + "xml/";
            ProjectImage_Path = Resouce_Root + "project/";
            BCFImage_Path = Resouce_Root + "bcf/";
            ProjectSnaphotImage_Path = Resouce_Root + "project/";
            Package_Path = Resouce_Root + "package/";
            Workspace_Path = Resouce_Root + "workspace/";
            Icon_Default = UserIcon_Path + "default.png";
            Image_Default = UserIcon_Path + "logo.png";
        }






        public static bool IsDebug = true;

        public static string None = "None";
        public static string Default = "defualt";
        public static string Registered = "registered";
        public static string Platform = "web";
        public static string DateString = "yyyy-MM-dd";
        public static string DateTimeString = "yyyy-MM-dd HH:mm";
        public static string DateTimeStringLogger = "yyyy-MM-dd HH:mm:ss";

        public static float GetCodeCountDown = 60f;
        public static int SecurityCodeLenght = 6;
        public static int InputContentLimit = 120;
        public static int MAX_DEVICE_NUMBER = 100;
        public static int MAX_PAGE_HISTORY = 20;
        public static long MOBILE_MASKHASH = 731;

        public static string Download_Path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        public static CultureInfo CultureInfo = new CultureInfo("en-US");

        // Configuration PlayerPref
        public static string Pref_SavedUser = "SavedProfile";
        public static string Pref_Localization = "Localization";
        public static string Pref_RememberOnDevice = "RememberOnDevice";

        // Third Party API
        public static string API_ExternalIP = "https://icanhazip.com";
        public static string API_MapboxProxy = "https://mapproxy.darkspede.space/OnlineMapsCORS/redirectFast.php?";
        public static string API_MapboxGeoCoding = "https://api.mapbox.com/geocoding/v5/mapbox.places/";
        public static string API_MapboxAccessToken = "pk.eyJ1IjoieXVoZTA5MjUiLCJhIjoiY2tkbnJobWM2MHY2ZTJycWltNG1lbm5xNyJ9.OPHhWDcFai-mdAqNLzhATA";
        public static string API_GoogleStaticMap = "https://maps.googleapis.com/maps/api/staticmap?";
        public static string API_GoogleAPIKey = "AIzaSyCXLPjZx1SNr0m7jFydmY_h3dDKtLNqH7w";  // this is from account of sycamre.master, need to replace with metabim api key

        // 
        public static string Download_RevitPlugin = Package_Path + "revit_plugin.zip";

        // External JSLIB
        public static string JavascriptCallback = "JavescriptCallBack";


        #region Online Map API Config


        #endregion



        // Controller 
        public static string Instance_ResourceHolder = "ResourceHolder";
        public static string Instance_BIMMaterialHolder = "BIMMaterialHolder";
        public static string Instance_MetaGraphController = "MetaGraphController";
        public static string Instance_LocalizationController = "LocalizationController";
        public static string Instance_AppController = "AppController";
        public static string Instance_MapController = "MapController";
        public static string Instance_DataProxy = "DataProxy";








        #region API Targets





        //Admin


        public static string API_OnRequestSysLog = "api/admin/OnRequestSyslog.aspx?numberOfLogs=1";


        // System
        public static string API_OnRequestLoginByToken = "OnRequestLoginByToken.aspx";
        public static string API_OnRequestLoginByMobile = "OnRequestLoginByMobile.aspx";
        public static string API_OnRequestAuthCodeByMobile = "OnRequestAuthCodeByMobile.aspx";
        public static string API_OnRequestDownloadRevitPlugin = "OnRequestDownloadRevitPlugin.aspx";
        public static string API_OnProcessRequest = "OnProcessRequest.aspx";

        // Third Party API
        public static string API_OnRequestGeoCode = "OnRequestGeoCode.aspx";
        public static string API_OnRequestGeoLocation = "OnRequestGeoLocation.aspx";
        public static string API_OnRequestGeoSearch = "OnRequestGeoSearch.aspx";


        //  MetaBIM  Data API
        public static string API_AddProfile = "AddProfile.aspx";
        public static string API_ModifyProfile = "ModifyProfile.aspx";
        public static string API_GetProfileByGuid = "GetProfileByGuid.aspx";
        public static string API_GetProfiles = "GetProfiles.aspx";
        public static string API_DeleteProfileByGuid = "DeleteProfileByGuid.aspx";

        public static string API_AddElementZone = "AddElementZone.aspx";
        public static string API_ModifyElementZone = "ModifyElementZone.aspx";
        public static string API_GetElementZoneByGuid = "GetElementZoneByGuid.aspx";
        public static string API_GetElementZones = "GetElementZones.aspx";
        public static string API_DeleteElementZoneByGuid = "DeleteElementZoneByGuid.aspx";

        public static string API_AddWorkspace = "AddWorkspace.aspx";
        public static string API_ModifyWorkspace = "ModifyWorkspace.aspx";
        public static string API_GetWorkspaceByGuid = "GetWorkspaceByGuid.aspx";
        public static string API_GetWorkspaces = "GetWorkspaces.aspx";
        public static string API_DeleteWorkspaceByGuid = "DeleteWorkspaceByGuid.aspx";
        
        public static string API_OnRequestExampleWorkspace = "OnRequestExampleWorkspace.aspx";

        public static string API_AddVersion = "AddVersion.aspx";
        public static string API_ModifyVersion = "ModifyVersion.aspx";
        public static string API_GetVersionByGuid = "GetVersionByGuid.aspx";
        public static string API_GetVersions = "GetVersions.aspx";
        public static string API_DeleteVersionByGuid = "DeleteVersionByGuid.aspx";


        public static string API_AddRequest = "AddRequest.aspx";
        public static string API_ModifyRequest = "ModifyRequest.aspx";
        public static string API_GetRequestByGuid = "GetRequestByGuid.aspx";
        public static string API_GetRequests = "GetRequests.aspx";
        public static string API_DeleteRequestByGuid = "DeleteRequestByGuid.aspx";

        public static string API_AddBimObject = "AddBimObject.aspx";
        public static string API_ModifyBimObject = "ModifyBimObject.aspx";
        public static string API_GetBimObjectByGuid = "GetBimObjectByGuid.aspx";
        public static string API_GetBimObjects = "GetBimObjects.aspx";
        public static string API_DeleteBimObjectByGuid = "DeleteBimObjectByGuid.aspx";

        public static string API_AddElementSplit = "AddElementSplit.aspx";
        public static string API_ModifyElementSplit = "ModifyElementSplit.aspx";
        public static string API_GetElementSplitByGuid = "GetElementSplitByGuid.aspx";
        public static string API_GetElementSplits = "GetElementSplits.aspx";
        public static string API_DeleteElementSplitByGuid = "DeleteElementSplitByGuid.aspx";

        public static string API_AddDocument = "AddDocument.aspx";
        public static string API_ModifyDocument = "ModifyDocument.aspx";
        public static string API_GetDocumentByGuid = "GetDocumentByGuid.aspx";
        public static string API_GetDocuments = "GetDocuments.aspx";
        public static string API_DeleteDocumentByGuid = "DeleteDocumentByGuid.aspx";


        public static string API_AddAnnotation = "AddAnnotation.aspx";
        public static string API_ModifyAnnotation = "ModifyAnnotation.aspx";
        public static string API_GetAnnotationByGuid = "GetAnnotationByGuid.aspx";
        public static string API_GetAnnotations = "GetAnnotations.aspx";
        public static string API_DeleteAnnotationByGuid = "DeleteAnnotationByGuid.aspx";


        public static string API_AddComment = "AddComment.aspx";
        public static string API_ModifyComment = "ModifyComment.aspx";
        public static string API_GetCommentByGuid = "GetCommentByGuid.aspx";
        public static string API_GetComments = "GetComments.aspx";
        public static string API_DeleteCommentByGuid = "DeleteCommentByGuid.aspx";


        public static string API_AddSupplier = "AddSupplier.aspx";
        public static string API_ModifySupplier = "ModifySupplier.aspx";
        public static string API_GetSupplierByGuid = "GetSupplierByGuid.aspx";
        public static string API_GetSuppliers = "GetSuppliers.aspx";
        public static string API_DeleteSupplierByGuid = "DeleteSupplierByGuid.aspx";


        public static string API_AddEasycarbonProject = "AddEasycarbonProject.aspx";
        public static string API_ModifyEasycarbonProject = "ModifyEasycarbonProject.aspx";
        public static string API_GetEasycarbonProjectByGuid = "GetEasycarbonProjectByGuid.aspx";
        public static string API_GetEasycarbonProjects = "GetEasycarbonProjects.aspx";
        public static string API_DeleteEasycarbonProjectByGuid = "DeleteEasycarbonProjectByGuid.aspx";


        public static string API_AddRemoteCommand = "AddRemoteCommand.aspx";
        public static string API_ModifyRemoteCommand = "ModifyRemoteCommand.aspx";
        public static string API_GetRemoteCommandByGuid = "GetRemoteCommandByGuid.aspx";
        public static string API_GetRemoteCommands = "GetRemoteCommands.aspx";
        public static string API_DeleteRemoteCommandByGuid = "DeleteRemoteCommandByGuid.aspx";


        public static string API_AddCustomStructure = "AddCustomStructure.aspx";
        public static string API_ModifyCustomStructure = "ModifyCustomStructure.aspx";
        public static string API_GetCustomStructureByGuid = "GetCustomStructureByGuid.aspx";
        public static string API_GetCustomStructures = "GetCustomStructures.aspx";
        public static string API_DeleteCustomStructureByGuid = "DeleteCustomStructureByGuid.aspx";

        public static string API_OnRequestUniclassUpdate = "OnRequestUniclassUpdate.aspx";
        public static string API_OnRequestIfcclassUpdate = "OnRequestIfcclassUpdate.aspx";
        public static string API_OnRequestEpicClassUpdate = "OnRequestEpicClassUpdate.aspx";




        /* Old models */
        public static string API_AddUser = "AddUser.aspx";
        public static string API_ModifyUser = "ModifyUser.aspx";
        public static string API_GetUserByGuid = "GetUserByGuid.aspx";
        public static string API_GetUsers = "GetUsers.aspx";
        public static string API_DeleteUserByGuid = "DeleteUserByGuid.aspx";

        public static string API_AddProject = "AddProject.aspx";
        public static string API_ModifyProject = "ModifyProject.aspx";
        public static string API_GetProjectByGuid = "GetProjectByGuid.aspx";
        public static string API_GetProjects = "GetProjects.aspx";
        public static string API_DeleteProjectByGuid = "DeleteProjectByGuid.aspx";
        public static string API_OnRequestGetCollaboratingProject = "OnRequestGetCollaboratingProject.aspx";

        public static string API_AddBCF = "AddBCF.aspx";
        public static string API_ModifyBCF = "ModifyBCF.aspx";
        public static string API_GetBCFByGuid = "GetBCFByGuid.aspx";
        public static string API_GetBCFs = "GetBCFs.aspx";
        public static string API_DeleteBCFByGuid = "DeleteBCFByGuid.aspx";
        public static string API_BCFImageUpload = "OnRequestBCFImageUpload.aspx";
        public static string API_ProjectSnaphotImageUpload = "OnRequestProjectSnaphotUpload.aspx";


        public static string API_AddConvertRequest = "AddConvertRequest.aspx";
        public static string API_ModifyConvertRequest = "ModifyConvertRequest.aspx";
        public static string API_GetConvertRequestByGuid = "GetConvertRequestByGuid.aspx";
        public static string API_GetConvertRequests = "GetConvertRequests.aspx";
        public static string API_DeleteConvertRequestByGuid = "DeleteConvertRequestByGuid.aspx";

        public static string API_OnRequestSendCollaborationInvite = "OnRequestSendCollaborationInvite.aspx";
        public static string API_ModifyCollaboration = "ModifyCollaboration.aspx";
        public static string API_GetCollaborationByGuid = "GetCollaborationByGuid.aspx";
        public static string API_GetCollaboration = "GetCollaboration.aspx";
        public static string API_DeleteCollaborationByGuid = "DeleteCollaborationByGuid.aspx";


        public static string API_AddVersionUpdate = "AddVersionUpdate.aspx";
        public static string API_ModifyVersionUpdate = "ModifyVersionUpdate.aspx";
        public static string API_GetVersionUpdateByGuid = "GetVersionUpdateByGuid.aspx";
        public static string API_GetVersionUpdates = "GetVersionUpdates.aspx";
        public static string API_DeleteVersionUpdateByGuid = "DeleteVersionUpdateByGuid.aspx";


        public static string API_OnUploadModelVersionNative = "OnUploadModelVersionNative.aspx";



        #endregion
        //-------------------------------------------------------------------------------------------------------------------------------------------------



    }
}

