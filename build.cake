//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////
#addin nuget:?package=Cake.FileHelpers
#tool nuget:?package=Spellchecker
var target = Argument("target", "Default");
var reposistoryPath=MakeAbsolute(Directory("../"));
var cireports = Argument("cireports", "../cireports");
var platform=Argument<string>("platform","");
var sourcebranch=Argument<string>("branch","");
var buildStatus = true;
var exitcode=0;
var filePath="";
//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////


Task("build")
    .Does(() =>
{   
var directories = GetSubDirectories(reposistoryPath);
foreach(var repository in directories)
    {
	 if(!repository.ToString().Contains("ug_spellchecker")&&!repository.ToString().Contains("cireports"))
	 {
	   filePath=repository.ToString();
	 }
	}
	Information(filePath);
    try {
             exitcode=StartProcess("DocumentSpellChecker.exe",new ProcessSettings{ Arguments = "/IsCIOperation:true /platform:"+platform+" /filepath:"+filePath});
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
CopyFileToDirectory(filePath+"/spellcheckreport.htm", cireports);
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
