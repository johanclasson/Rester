# Rester

Have you ever wished for a REST client that is easy to use on your windows phone? That is also available on your Windows computer? With the same configured data? Then Rester is the rest client for you.

Rester is a multi-purpose REST client. It lets you configure actions for your services, so you can then invoke one of those by just pressing a button. If you use Rester on many devices which are connected to the same Microsoft account, your configured service actions will be kept up to date on all of them. All without any need for additional setup.

Rester is an Open Source project. If you miss a feature and have the right skills, you can implement it yourself.

You can find Rester in the Microsoft Store.

[![Get it on Windows 10](https://assets.windowsphone.com/d86ab9b4-2f3d-4a94-92f8-1598073e7343/English_Get_it_Win_10_InvariantCulture_Default.png)](https://www.microsoft.com/store/apps/9nblggh5qpgx).

## Features

* **Visual Feedback:** Get indications of when a request is started and when it has ended.
* **Synchronize Configured Services:** Data will be synchronized between all your devices which are connected to the same Microsoft Account.
* **Detailed Log:** All requests and responses are logged with detailed information about the outcome. (*)
* **Multiple Service Requests:** Run many service calls in parallel.
* **Export Configured Service Actions:** This can be used for backup purposes, or if you want to share your configuration with other people.
* **Import Service Actions:** When importing data you can choose to add the configured services to your collection of services, or to let them replace your collection entirely.
* **Open Source:** If you miss a feature, you can contribute it yourself. 
* **Responsive design:** No matter what your screen size is, Rester will try to layout its buttons in an efficient manner.

(* Se current limitations)

## How to Configure

Rester contains a number of *service configurations*. A service configuration contains the root uri to a service, together with authentication options (*).

A service configuration contains a number of *action groups*. An action group is just a container so that related *actions* can be grouped together.

An action contains the request details such as URI path, method, body and content type.

## Current Limitations

The implementation of Rester is still in progress. Because of that, you will find that some features are missing. The most important ones that are not done (*yet!*) are:

* Logs are not persisted.
* Although detailed information is logged, it is not displayed in the user interface.
* Services, action groups and actions cannot be reordered. This can be a mayor pain, but there is a workaround.
	1. Export your data.
	2. Unzip the .rdb-file, for example with [7-zip](http://www.7-zip.org/).
	3. Edit the json-data with a text editor.
	4. Zip the file, and make sure to select GZip as compression method.
	5. Import your data.
* Authentication is not supported. For Basic Authentication there is a work around. Put the username and password in the URL like this: http://{username}:{password}@myservice.com/mypath.
* Data synchronization speed is quite slow. Expect that it will take around 5-15 minutes (or even longer in some cases) before all devices have been updated.

## How Can You Contribute

* **GitHub Issues:** Please do not hesitate to create an [issue](https://github.com/johanclasson/Rester/issues) if you encounter a bug, or if you would like that something should be working differently. Writing comments on already reported issues also help!
* **Trello:**  You can write comments on-, and vote for features on the public [Trello board](https://trello.com/b/f19z4Wwu).
* **Pull Request:** Please do send pull requests! :) And more precise, since Rester is licensed under GNUv2 you have to contribute back any changes you make to the original software.
