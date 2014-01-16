/*
       Licensed to the Apache Software Foundation (ASF) under one
       or more contributor license agreements.  See the NOTICE file
       distributed with this work for additional information
       regarding copyright ownership.  The ASF licenses this file
       to you under the Apache License, Version 2.0 (the
       "License"); you may not use this file except in compliance
       with the License.  You may obtain a copy of the License at

         http://www.apache.org/licenses/LICENSE-2.0

       Unless required by applicable law or agreed to in writing,
       software distributed under the License is distributed on an
       "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
       KIND, either express or implied.  See the License for the
       specific language governing permissions and limitations
       under the License.
*/

/*
 * create a xface/wp8 project
 *
 * USAGE
 *  ./create [path package activity]

    ./bin/create.bat --shared C:\Users\Me\MyTestProj "test.proj" "TestProject"
 */


var fso=WScript.CreateObject("Scripting.FileSystemObject");
var wscript_shell = WScript.CreateObject("WScript.Shell");
// working dir
var templateRoot = WScript.ScriptFullName.split('\\create.js').join('');
var repoRoot =  fso.GetParentFolderName(templateRoot);
var args = WScript.Arguments;
var templatePath = "\\template"
var destPath;

//  Link directly against the shared copy of the xFaceLib instead of a copy of it, default false = copy, true = link share
var shared = false;

Log("templateRoot = " + templateRoot);
Log("repoRoot = " + repoRoot);


function Usage() {
    Log("Usage: create [--shared] PathToNewProject [ PackageName AppName ]");
    Log("	--shared (optional): Link directly against the shared copy of the xFaceLib instead of a copy of it.");
    Log("    PathToNewProject : The path to where you wish to create the project");
    Log("    PackageName      : The namespace for the project (default is xFace.Example)")
    Log("    AppName          : The name of the application (default is xFaceProj)");
    Log("examples:");
    Log("    create C:\\Users\\anonymous\\Desktop\\MyProject");
    Log("    create C:\\Users\\anonymous\\Desktop\\MyProject io.xFace.Example AnApp");
}

// logs messaged to stdout and stderr
function Log(msg, error) {
    if (error) {
        WScript.StdErr.WriteLine(msg);
    }
    else {
        WScript.StdOut.WriteLine(msg);
    }
}

var ForReading = 1, ForWriting = 2, ForAppending = 8;
var TristateUseDefault = -2, TristateTrue = -1, TristateFalse = 0;

function read(filename) {
    var f=fso.OpenTextFile(filename, 1,2);
    var s=f.ReadAll();
    f.Close();
    return s;
}

function write(filename, contents) {
    var f=fso.OpenTextFile(filename, ForWriting, TristateTrue);
    f.Write(contents);
    f.Close();
}

function replaceInFile(filename, regexp, replacement) {
    write(filename,read(filename).replace(regexp,replacement));
}

// deletes file if it exists
function deleteFileIfExists(path) {
    if(fso.FileExists(path)) {
        fso.DeleteFile(path);
   }
}


// executes a commmand in the shell
function exec(command) {
    var oShell=wscript_shell.Exec(command);
    while (oShell.Status == 0) {
        WScript.sleep(100);
    }
}

// executes a commmand in the shell
function exec_verbose(command) {
    //Log("Command: " + command);
    var oShell=wscript_shell.Exec(command);
    while (oShell.Status == 0) {
        //Wait a little bit so we're not super looping
        WScript.sleep(100);
        //Print any stdout output from the script
        if (!oShell.StdOut.AtEndOfStream) {
            var line = oShell.StdOut.ReadLine();
            Log(line);
        }
    }
    //Check to make sure our scripts did not encounter an error
    if (!oShell.StdErr.AtEndOfStream) {
        var line = oShell.StdErr.ReadAll();
        Log("ERROR: command failed in create.js : " + command);
        Log(line, true);
        WScript.Quit(1);
    }
}

//generate guid for the project
function genGuid() {
    var TypeLib = WScript.CreateObject("Scriptlet.TypeLib");
    strGuid = TypeLib.Guid.split("}")[0]; // there is extra crap after the } that is causing file streams to break, probably an EOF ... 
    strGuid = strGuid.replace(/[\{\}]/g,""); 
    return strGuid;
}

// creates new project in path, with the given package and app name
function create(path, namespace, name) {
    Log("Creating xFace-WP8 Project:");
    Log("\tApp Name : " + name);
    Log("\tNamespace : " + namespace);
    Log("\tPath : " + path);

    // Copy the template source files to the new destination
    fso.CopyFolder(templateRoot + templatePath, path);

    // copy the xFaceLib\xFaceLib\xface.js file to project
    fso.CopyFile(repoRoot +'\\xFaceLib\\xFaceLib\\xface.js', path + "\\xface3\\helloxface\\" );

    if(!shared) {
        // copy over xFaceLib files & create xFaceLib folder
        fso.CreateFolder(path + "\\xFaceLib");
        fso.CopyFolder(repoRoot + "\\xFaceLib", path + "\\xFaceLib");

        // copy the cordova-wp8\wp8\template\cordovalib files & create cordova-wp8\wp8\template\cordovalib folder
        fso.CreateFolder(path + "\\cordova-wp8");
        fso.CreateFolder(path + "\\cordova-wp8\\wp8");
        fso.CreateFolder(path + "\\cordova-wp8\\wp8\\template");
        fso.CreateFolder(path + "\\cordova-wp8\\wp8\\template\\cordovalib");
        fso.CopyFolder(repoRoot +'\\cordova-wp8\\wp8\\template\\cordovalib', path + "\\cordova-wp8\\wp8\\template\\cordovalib");
    }


    // remove template cruft
    deleteFileIfExists(path + "\\__PreviewImage.jpg");
    deleteFileIfExists(path + "\\__TemplateIcon.png");
    deleteFileIfExists(path + "\\MyTemplate.vstemplate");
    deleteFileIfExists(path + "\\__TemplateIcon.ico");

    var newProjGuid = genGuid();
    // replace the guid in the AppManifest
    replaceInFile(path + "\\Properties\\WMAppManifest.xml","$guid1$",newProjGuid);
    // replace safe-project-name in AppManifest
    replaceInFile(path + "\\Properties\\WMAppManifest.xml",/\$safeprojectname\$/g,name);
    replaceInFile(path + "\\Properties\\WMAppManifest.xml",/\$projectname\$/g,name);

    replaceInFile(path + "\\App.xaml",/\$safeprojectname\$/g,namespace);
    replaceInFile(path + "\\App.xaml.cs",/\$safeprojectname\$/g,namespace);

    replaceInFile(path + "\\LocalizedStrings.cs",/\$safeprojectname\$/g,namespace);
    replaceInFile(path + "\\Resources\\AppResources.Designer.cs",/\$safeprojectname\$/g,namespace);
    //shared need replace sln/.csproj add repoRoot
    if(shared) {
        replaceInFile(path + "\\xFaceSolution.sln",/..\\..\\xFaceLib/g,repoRoot + "\\xFaceLib");
        replaceInFile(path + "\\xFaceProj.csproj",/..\\..\\xFaceLib/g,repoRoot + "\\xFaceLib");
    } else {
        replaceInFile(path + "\\xFaceSolution.sln",/..\\..\\xFaceLib/g, "xFaceLib");
        replaceInFile(path + "\\xFaceProj.csproj",/..\\..\\xFaceLib/g, "xFaceLib");
    }
    replaceInFile(path + "\\xFaceProj.csproj",/\$safeprojectname\$/g,namespace);
    replaceInFile(path + "\\xFaceProj.csproj",/\$projectname\$/g,name);

    if (name != "xFaceProj") {
        var valid_name = name.replace(/(\.\s|\s\.|\s+|\.+)/g, '_');
        replaceInFile(path + "\\xFaceSolution.sln", /xFaceProj/g, valid_name);
        // rename project and solution
        exec('%comspec% /c ren ' + path + "\\xFaceSolution.sln " + valid_name + '.sln');
        exec('%comspec% /c ren ' + path + "\\xFaceProj.csproj " + valid_name + '.csproj');
    }

    //clean up any Bin/obj or other generated files
    exec('cscript "' + path + '\\cordova\\lib\\clean.js" //nologo');

    // delete any .user and .sou files if any
    if (fso.FolderExists(path)) {
        var proj_folder = fso.GetFolder(path);
        var proj_files = new Enumerator(proj_folder.Files);
        for (;!proj_files.atEnd(); proj_files.moveNext()) {
            if (fso.GetExtensionName(proj_files.item()) == 'user') {
                fso.DeleteFile(proj_files.item());
            } else if (fso.GetExtensionName(proj_files.item()) == 'sou') {
                fso.DeleteFile(proj_files.item());
            }
        }
    }

    Log("CREATE SUCCESS : " + path);

    // TODO:
    // index.html title set to project name?
}

// MAIN

if (args.Count() > 0) {
    // support help flags
    if (args(0) == "--help" || args(0) == "/?" ||
            args(0) == "help" || args(0) == "-help" || args(0) == "/help" || args(0) == "-h") {
        Usage();
        WScript.Quit(1);
    }
    
    //--shared
    if (args(0) == "--shared") {
        shared = true;
        if (args.Count() > 1) {
            destPath = args(1);
        } else {
            Usage();
            WScript.Quit(1);
        }
        
        if (fso.FolderExists(destPath)) {
            Log("Project directory already exists:", true);
            Log("\t" + destPath, true);
            Log("CREATE FAILED.", true);
            WScript.Quit(1);
        }
        var packageName = "xFace.Example";
        if (args.Count() > 2) {
            packageName = args(2);
        }

        var projName = "xFace";
        if (args.Count() > 3) {
            projName = args(3);
        }
    } else {
        destPath = args(0);
        if (fso.FolderExists(destPath)) {
            Log("Project directory already exists:", true);
            Log("\t" + destPath, true);
            Log("CREATE FAILED.", true);
            WScript.Quit(1);
        }
        var packageName = "xFace.Example";
        if (args.Count() > 1) {
            packageName = args(1);
        }

        var projName = "xFace";
        if (args.Count() > 2) {
            projName = args(2);
        }
    }

    create(destPath, packageName, projName);
}
else {
    Usage();
    WScript.Quit(1);
}

