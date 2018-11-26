//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////
#addin nuget:?package=Cake.FileHelpers
#tool nuget:?package=Syncfusion.Spellcheck.CI
var target = Argument("target", "Default");
var reposistoryPath=MakeAbsolute(Directory("../"));
#tool nuget:?package=Syncfusion.Content.DocumentValidation.CI
var cireports = Argument("cireports", "../cireports");
var platform=Argument<string>("platform","");
var sourcebranch=Argument<string>("branch","");
var targetBranch=Argument<string>("targetbranch","");
var buildStatus = true;
var isSpellingError=0;
var isDocumentvalidationError=0;
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
 CopyFiles("./tools/Syncfusion.Content.DocumentValidation.CI/Syncfusion.Content.DocumentValidation.CI/content/*", "./");
 CopyFiles("./tools/Syncfusion.Content.DocumentValidation.CI/Syncfusion.Content.DocumentValidation.CI/lib/*", "./");
 EnsureDirectoryExists("./Templates");
 CopyFiles("./tools/Syncfusion.Content.DocumentValidation.CI/Syncfusion.Content.DocumentValidation.CI/Templates/*", "./Templates");
  
  
  var directories = GetSubDirectories(reposistoryPath);
  foreach(var repository in directories)
    {
	 if(!repository.ToString().Contains("ug_spellchecker")&&!repository.ToString().Contains("cireports"))
	 {
	  sourcefolder=repository.ToString();
	 }
	}
    try {
        //Code to run spellchecker tool
        isSpellingError=StartProcess("./tools/DocumentSpellChecker.exe",new ProcessSettings{ Arguments = "/IsCIOperation:true /platform:"+platform+" /branch:"+sourcebranch+" /sourcefolder:"+sourcefolder});
        
        //Code to run the Document validation tool
        repositoryName =reposistoryPath.ToString().Split('/')[3].Split('@')[0];
        Information(reposistoryPath+"/Spell-Checker/ "+repositoryName+" "+targetBranch);
        isDocumentvalidationError=StartProcess("./DocumentationValidation.exe",new ProcessSettings{ Arguments = reposistoryPath+"/Spell-Checker/ "+repositoryName+" "+targetBranch});
	}
	catch(Exception ex)
	{        
		buildStatus = false;
	}
	if(isSpellingError==0 && isDocumentvalidationError==0 && buildStatus) {    
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
