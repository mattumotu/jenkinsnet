![JenkinsNET Logo](https://raw.githubusercontent.com/mattumotu/jenkinsnetclient/master/jenkinsnetclient.png "JenkinsNETClient Logo") 

[![Build status](https://ci.appveyor.com/api/projects/status/61ei2xqdhwf1i8ap/branch/master?svg=true)](https://ci.appveyor.com/project/mattumotu/jenkinsnet/branch/master)
[![Coverage Status](https://coveralls.io/repos/github/mattumotu/jenkinsnet/badge.svg?branch=master)](https://coveralls.io/github/mattumotu/jenkinsnet?branch=master)

Allows for easy and simple C# interaction with Jenkins.

## Basics

First you need a connection to a jenkins server, just pass it your jenkins instance's url.
```cs
JenkinsConnection myConn = new JenkinsConnection("myjenkins");
```

If Jenkins is secuired then you will have to provide a username and api token:
```cs
JenkinsConnection myConn = new JenkinsConnection("myjenkins", "username", "apitoken");
```

We can get a list of Views and Jobs from a JenkinsServer
```cs
var myJenkins = new JenkinsServer(myConn);
List<JenkinsView> views = myJenkins.Views();
List<JenkinsJob> jobs = myJenkins.Jobs();
```

## Playing with views
We can easily create a new view ...
```cs
var newView = new JenkinsView(myConn, "hudson.model.ListView", "Name of my new view");
if(newView.Exists()) 
{
  throw new exception("View already exists on jenkins");
}
newView.Create(); // Create on jenkins
if(!newView.Exists()) 
{
  throw new exception("View doesn't exist on jenkins - Create must have failed");
}
```

... or delete an existing view
```cs
JenkinsView existingView = views[0];
existingView.Delete(); // Delete from jenkins
if(existingView.Exists()) 
{
  throw new exception("View still exists on jenkins - delete failed");
}
```

## Playing with Jobs
```cs
var newJob = new JenkinsJob(myConn, "hudson.model.FreeStyleProject", "name of new job");
if(newJob.Exists()) 
{
  throw new exception("job already exists on jenkins");
}
newJob.Create(); // Create job on jenkins
if(!newJob.Exists()) 
{
  throw new exception("job doesn't exist on jenkins after call to create");
}
newJob.Delete();
if(newJob.Exists()) 
{
  throw new exception("job still exists on jenkins after call to delete");
}           
```
## Jobs and Views 

Adding an existing job to a view ...
```cs
JenkinsJob existingJob = Jobs[0];
JenkinsView existingView = views[0];
if(!existingView.Contains(existingJob))
{
  existingView.Add(existingJob);
}
if(!existingView.Contains(existingJob)) 
{
  throw new exception("Add failed");
}
```
... and removing it again.
```cs
existingView.Remove(existingJob);
if(existingView.Contains(existingJob)) 
{
  throw new exception("job still on view - remove failed");
}
```

## Getting and Setting config.xml

Getting the config xml for a job
```cs
JenkinsJob existingJob = Jobs[0];
string configXml = existingJob.Config;
```

and setting it
```cs
JenkinsJob existingJob = Jobs[0];
existingJob.Config = xml
```
