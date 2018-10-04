//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////
#addin nuget:?package=Cake.FileHelpers
#tool nuget:?package=Syncfusion.Spellcheck.CI
var target = Argument("target", "Default");
var reposistoryPath=MakeAbsolute(Directory("../"));
#tool nuget:?package=Syncfusion.Content.ImageValidation.CI
var cireports = Argument("cireports", "../cireports");
var platform=Argument<string>("platform","");
var sourcebranch=Argument<string>("branch","");
var buildStatus = true;
var isSpellingError=0;
var isImagevalidationError=0;
var sourcefolder="";
var repositoryName="";

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////


Task("build")
    .Does(() =>
{   
 CopyFiles("./tools/syncfusion.spellcheck.ci/Syncfusion.Spellcheck.CI/content/*", "./tools");
 CopyFiles("./tools/syncfusion.spellcheck.ci/Syncfusion.Spellcheck.CI/lib/*", "./tools");
 CopyFiles("./tools/syncfusion.content.imagevalidation.ci/Syncfusion.Content.imagevalidation.CI/content/*", "./");
 CopyFiles("./tools/syncfusion.content.imagevalidation.ci/Syncfusion.Content.imagevalidation.CI/lib/*", "./");
 EnsureDirectoryExists("./Templates");
 CopyFiles("./tools/syncfusion.content.imagevalidation.ci/Syncfusion.Content.imagevalidation.CI/Templates/*", "./Templates");
  
  
  var directories = GetSubDirectories(reposistoryPath);
  foreach(var repository in directories)
    {
	 if(!repository.ToString().Contains("ug_spellchecker")&&!repository.ToString().Contains("cireports"))
	 {
	  sourcefolder=repository.ToString();
	 }
	}
    try {
            isSpellingError=StartProcess("./tools/DocumentSpellChecker.exe",new ProcessSettings{ Arguments = "/IsCIOperation:true /platform:"+platform+" /branch:"+sourcebranch+" /sourcefolder:"+sourcefolder});
	    
			repositoryName =reposistoryPath.ToString().Split('/')[3];
			 
			isImagevalidationError=StartProcess("./ImageValidator.exe",new ProcessSettings{ Arguments = reposistoryPath+"/Spell-Checker/"+" "+repositoryName});
		}
	catch(Exception ex)
	{        
		buildStatus = false;
	}
	if(isSpellingError==0 && isImagevalidationError==0 && buildStatus) {    
		Information("Compilation successfull");
		RunTarget("CopyFile");
	} 
	else {   
		throw new Exception(String.Format("Please fix the project compilation failures"));  
	}
});

Task("CopyFile")
.Does(() =>
{
		if (!DirectoryExists(cireports))
		{			
			CreateDirectory(cireports);
		}

		EnsureDirectoryExists(cireports+"/spellcheck/");
		
		if (FileExists(cireports+"/spellcheckreport.htm"))
		{
			MoveFileToDirectory(cireports+"/spellcheckreport.htm", cireports+"/spellcheck/");
		}
		
		EnsureDirectoryExists(cireports+"/imagevalidation/");
		
		var files = GetFiles("./*.html");
		CopyFiles(files,cireports+"/imagevalidation/");
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("build");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
