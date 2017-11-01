![JenkinsNET Logo](https://raw.githubusercontent.com/mattumotu/jenkinsnet/master/jenkinsnet.png "JenkinsNET Logo") 

Allows for easy and simple C# interaction with Jenkins.

## Basics

First you need a connection to a jenkins server, just pass it your jenkins instance's url.
```cs
JenkinsConnection myConn = new JenkinsConnection("myjenkins");
```

We can get a list of Views and Jobs from a JenkinsServer
```cs
JenkinsConnection myConn = new JenkinsConnection("myjenkins");
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
