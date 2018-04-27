//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////
#addin nuget:?package=Cake.FileHelpers
#tool nuget:?package=Syncfusion.Spellcheck.CI
var target = Argument("target", "Default");
#tool nuget:?package=Syncfusion.Content.Validation.CI
var reposistoryPath=MakeAbsolute(Directory("../"));
var cireports = Argument("cireports", "../cireports");
var platform=Argument<string>("platform","");
var sourcebranch=Argument<string>("branch","");
var Targetbranch=Argument<string>("targetbranch","");
var buildStatus = true;
var exitcode=0;
var fileValidator=0;
var sourcefolder="";

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
  CopyFiles("./tools/syncfusion.content.validation.ci/Syncfusion.Content.Validation.CI/content/*", "./");
 CopyFiles("./tools/syncfusion.content.validation.ci/Syncfusion.Content.Validation.CI/lib/*", "./");
var directories = GetSubDirectories(reposistoryPath);
foreach(var repository in directories)
    {
	 if(!repository.ToString().Contains("ug_spellchecker")&&!repository.ToString().Contains("cireports"))
	 {
	   sourcefolder=repository.ToString();
	 }
	}
    try {
             exitcode=StartProcess("./tools/DocumentSpellChecker.exe",new ProcessSettings{ Arguments = "/IsCIOperation:true /platform:"+platform+" /branch:"+sourcebranch+" /sourcefolder:"+sourcefolder});
			 var filePath = @reposistoryPath + "/Spell-Checker/" +platform;
             fileValidator = StartProcess("./FilePathValidator.exe",new ProcessSettings{ Arguments =filePath+" "+Targetbranch});
		}
	catch(Exception ex)
	{        
		buildStatus = false;
	}
	if(exitcode==0 && fileValidator==0 && buildStatus) {    
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
	if(FileExists("./MissingFilesInToc.txt"))
	{
		MoveFileToDirectory("./MissingFilesInToc.txt", cireports+"/spellcheck/");
	}
	
	if(FileExists("./AddFilesToToc.txt"))
	{
		MoveFileToDirectory("./AddFilesToToc.txt", cireports+"/spellcheck/");
	}
	
	if(FileExists("./DummyVersionInToc.txt"))
	{
		MoveFileToDirectory("./DummyVersionInToc.txt", cireports+"/spellcheck/");
	}

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
