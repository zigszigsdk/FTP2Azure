FTP2Azure
=========

FTP for Azure worker. Stores FTP files on the blob store. Works with .NET 4.5+

## Quickguide

### Requires:
```
  Microsoft.WindowsAzure.ServiceRuntime 2.2.0.0
  Microsoft.WindowsAzure.Storage 2.1.0.0
```

### Azure Worker
Make an azure worker, and add the content of the source folder. The following boilerplate should help you get started:
```c#
    public class WorkerRole : RoleEntryPoint
    {
        private FtpServer _server;

        public override bool OnStart()
        {
            if (_server == null)
                _server = new FtpServer();

            _server.NewConnection += ServerNewConnection;
            _server.UserLogoutEvent += ServerLogoutEvent;
            _server.errorHandler += ErrorHandle;

            return base.OnStart();
        }

        public override void Run()
        {
            while (true)
            {
                if (_server.Started)
                {
                    Thread.Sleep(10000);
                    Trace.WriteLine("Server is alive.", "Information");
                }
                else
                {
                    _server.Start();
                    Trace.WriteLine("Server starting.", "Control");
                }
            }
        }

        void ServerNewConnection(int nId)
        {
            Trace.WriteLine(String.Format("Connection {0} accepted", nId), "Connection");
        }

        void ServerLogoutEvent(string username)
        {
            Trace.WriteLine(String.Format("user {0} logged out", username), "Connection");
        }

        void ErrorHandle(Exception e) 
        {
            Trace.WriteLine("Error in FTP: " + e.Message);
            throw e;
        }
    }
```

### Implementation own an Account Manager
For use own an Account Manager you should implement interface IAccountManager and pass him into FtpServer constructor as a parameter.
```c#
    class AcccountManager : IAccountManager
    {
        public int UserNum { get; }
        public int LoadConfigration()
        {
            // Implement your own logic of loading accounts from another source, e.g. database, Web API, external config file.
        }

        public bool CheckAccount(string username, string password)
        {
            // Implement your own logic of checking  account and password
        }

        public string GetPassword(string username)
        {
            // Implement your own logic of getting password by username
        }
    }
```

### Service Config:
```xml
      <Setting name="FTP2Azure.Mode" value="Debug" /> <!--Debug or Live-->
      <Setting name="FTP2Azure.FtpAccount" value="(testuser:testpassword)(user2:pass)" /> <!-- config your users here-->
      <Setting name="FTP2Azure.FtpServerHost" value="localhost" /> <!-- [yourapp].cloudapp.net in cloud -->
      <Setting name="FTP2Azure.MaxIdleSeconds" value="120" />
      <Setting name="FTP2Azure.QueueNotification" value="false" />
      <Setting name="FTP2Azure.ConnectionEncoding" value="UTF8" />
      <Setting name="FTP2Azure.MaxClients" value="5" />
      <Setting name="FTP2Azure.StorageAccount" value="UseDevelopmentStorage=true" />
```
Note that the username will be the name of the blob storage container.

### Endpoints
```xml
      <InputEndpoint name="FTP2Azure.Command" protocol="tcp" port="21" localPort="9003" /> 
      <InputEndpoint name="FTP2Azure.Passive" protocol="tcp" port="9001" localPort="9002" />
```

## Known issues
 - Passive mode does not currently work (the client will have to forward ports in order to connect and stay in active)
 - usernames and passwords set in the service cofig file cannot contain the following characters: `(`, `)` and `:`

## Improvement ideas
- keep the decoding logic solely inside of FtpSocketHandler and only pass FtpConnectionObject the full string message once an end-of-line is reached.


## Credits
 - Mohammed Habeeb for the original version of [C# FTP Server](http://www.codeguru.com/csharp/csharp/cs_internet/desktopapplications/article.php/c13163/Simple-FTP-Demo-Application-Using-CNET-20.htm)
 - [FTP2Azure codeplex team](http://ftp2azure.codeplex.com/team/view), for the previous versions.
 - On github: 
    - [ryandle](https://github.com/ryandle)
    - [the-tatarinov-av](https://github.com/the-tatarinov-av)
    - [basjem](https://github.com/basjem)
