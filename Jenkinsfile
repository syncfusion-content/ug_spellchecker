node('content')
{ 
String platform='uwp';
   try
	{   
		//Clone scm repository in Workspace source directory
		stage ('Checkout')   
	    { 
	    dir('Spell-Checker') 
           {
		     checkout scm
		    }
			 
		   //Checkout the ug_spellchecker Source
	  checkout([$class: 'GitSCM', branches: [[name: '*/development']], doGenerateSubmoduleConfigurations: false, extensions: [[$class: 'RelativeTargetDirectory', relativeTargetDir: 'ug_spellchecker']], submoduleCfg: [], userRemoteConfigs: [[credentialsId: env.gitlabCredentialId, url: 'https://gitlab.syncfusion.com/testgroup/ug_spellchecker.git']]])
		 
	  }
	  //method to get  content
	  changeLogs()
	}
	
    catch(Exception e)
    {
		currentBuild.result = 'FAILURE'
    } 

if(currentBuild.result != 'FAILURE')
{ 
	stage 'Build Source'
	try
	{
	    gitlabCommitStatus("Build")
		{
		bat 'powershell.exe -ExecutionPolicy ByPass -File '+env.WORKSPACE+"/ug_spellchecker/build.ps1 -Script "+env.WORKSPACE+"/ug_spellchecker/build.cake -Target build -Platform \""+platform+"\" -Branch "+env.gitlabSourceBranch
	 	}
    }
	 catch(Exception e) 
    {
		currentBuild.result = 'FAILURE'
    }
}	

	stage 'Delete Workspace'
	
		def files = findFiles(glob: '**/cireports/**/*.*')      
        
    if(files.size() > 0) 		
    { 		
         archiveArtifacts artifacts: 'cireports/', excludes: null 	
         //currentBuild.result = 'FAILURE'		 
    }
	   // step([$class: 'WsCleanup'])	
}
@NonCPS
def changeLogs(){
try{
     def Content=""; 	      
	 def changeLogSets = currentBuild.changeSets
for (int i = 0; i < changeLogSets.size(); i++) {
    def entries = changeLogSets[i].items
    for (int j = 0; j < entries.length; j++) {
        def entry = entries[j]
        def files = new ArrayList(entry.affectedFiles)
        for (int k = 0; k < files.size(); k++) {
            def file = files[k]
            echo "${file.path}"
            Content+= env.WORKSPACE+"\\Spell-Checker\\"+"${file.path}"+"\r\n";
        }
    }
}
	
    if (Content) {  
       writeFile file: env.WORKSPACE+"/cireports/content.txt", text: Content
    }
    else  {
       writeFile file: env.WORKSPACE+"/cireports/content.txt", text: "No filepath found for this commit."
    }
}
	catch(Exception e)
	{
	currentBuild.result = 'SUCCESS'
	}
 return "Success"
}
