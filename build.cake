//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////
#addin nuget:?package=Cake.FileHelpers
#tool nuget:?package=Syncfusion.Spellcheck.CI
var target = Argument("target", "Default");
var reposistoryPath=MakeAbsolute(Directory("../"));
var cireports = Argument("cireports", "../cireports");
var platform=Argument<string>("platform","");
var sourcebranch=Argument<string>("branch","");
var buildStatus = true;
var exitcode=0;
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
	    }
	catch(Exception ex)
	{        
		buildStatus = false;
	}
	if(exitcode==0 && buildStatus) {    
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
