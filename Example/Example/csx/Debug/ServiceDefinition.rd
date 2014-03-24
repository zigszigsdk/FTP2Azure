<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="Example" generation="1" functional="0" release="0" Id="57deb7bb-28bf-47d0-906e-92351ea431c9" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="ExampleGroup" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="FTPServerWorker:FTP2Azure.Command" protocol="tcp">
          <inToChannel>
            <lBChannelMoniker name="/Example/ExampleGroup/LB:FTPServerWorker:FTP2Azure.Command" />
          </inToChannel>
        </inPort>
        <inPort name="FTPServerWorker:FTP2Azure.Passive" protocol="tcp">
          <inToChannel>
            <lBChannelMoniker name="/Example/ExampleGroup/LB:FTPServerWorker:FTP2Azure.Passive" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="FTPServerWorker:FTP2Azure.ConnectionEncoding" defaultValue="">
          <maps>
            <mapMoniker name="/Example/ExampleGroup/MapFTPServerWorker:FTP2Azure.ConnectionEncoding" />
          </maps>
        </aCS>
        <aCS name="FTPServerWorker:FTP2Azure.FtpAccount" defaultValue="">
          <maps>
            <mapMoniker name="/Example/ExampleGroup/MapFTPServerWorker:FTP2Azure.FtpAccount" />
          </maps>
        </aCS>
        <aCS name="FTPServerWorker:FTP2Azure.FtpServerHost" defaultValue="">
          <maps>
            <mapMoniker name="/Example/ExampleGroup/MapFTPServerWorker:FTP2Azure.FtpServerHost" />
          </maps>
        </aCS>
        <aCS name="FTPServerWorker:FTP2Azure.MaxClients" defaultValue="">
          <maps>
            <mapMoniker name="/Example/ExampleGroup/MapFTPServerWorker:FTP2Azure.MaxClients" />
          </maps>
        </aCS>
        <aCS name="FTPServerWorker:FTP2Azure.MaxIdleSeconds" defaultValue="">
          <maps>
            <mapMoniker name="/Example/ExampleGroup/MapFTPServerWorker:FTP2Azure.MaxIdleSeconds" />
          </maps>
        </aCS>
        <aCS name="FTPServerWorker:FTP2Azure.Mode" defaultValue="">
          <maps>
            <mapMoniker name="/Example/ExampleGroup/MapFTPServerWorker:FTP2Azure.Mode" />
          </maps>
        </aCS>
        <aCS name="FTPServerWorker:FTP2Azure.QueueNotification" defaultValue="">
          <maps>
            <mapMoniker name="/Example/ExampleGroup/MapFTPServerWorker:FTP2Azure.QueueNotification" />
          </maps>
        </aCS>
        <aCS name="FTPServerWorker:FTP2Azure.StorageAccount" defaultValue="">
          <maps>
            <mapMoniker name="/Example/ExampleGroup/MapFTPServerWorker:FTP2Azure.StorageAccount" />
          </maps>
        </aCS>
        <aCS name="FTPServerWorker:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/Example/ExampleGroup/MapFTPServerWorker:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="FTPServerWorkerInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/Example/ExampleGroup/MapFTPServerWorkerInstances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:FTPServerWorker:FTP2Azure.Command">
          <toPorts>
            <inPortMoniker name="/Example/ExampleGroup/FTPServerWorker/FTP2Azure.Command" />
          </toPorts>
        </lBChannel>
        <lBChannel name="LB:FTPServerWorker:FTP2Azure.Passive">
          <toPorts>
            <inPortMoniker name="/Example/ExampleGroup/FTPServerWorker/FTP2Azure.Passive" />
          </toPorts>
        </lBChannel>
      </channels>
      <maps>
        <map name="MapFTPServerWorker:FTP2Azure.ConnectionEncoding" kind="Identity">
          <setting>
            <aCSMoniker name="/Example/ExampleGroup/FTPServerWorker/FTP2Azure.ConnectionEncoding" />
          </setting>
        </map>
        <map name="MapFTPServerWorker:FTP2Azure.FtpAccount" kind="Identity">
          <setting>
            <aCSMoniker name="/Example/ExampleGroup/FTPServerWorker/FTP2Azure.FtpAccount" />
          </setting>
        </map>
        <map name="MapFTPServerWorker:FTP2Azure.FtpServerHost" kind="Identity">
          <setting>
            <aCSMoniker name="/Example/ExampleGroup/FTPServerWorker/FTP2Azure.FtpServerHost" />
          </setting>
        </map>
        <map name="MapFTPServerWorker:FTP2Azure.MaxClients" kind="Identity">
          <setting>
            <aCSMoniker name="/Example/ExampleGroup/FTPServerWorker/FTP2Azure.MaxClients" />
          </setting>
        </map>
        <map name="MapFTPServerWorker:FTP2Azure.MaxIdleSeconds" kind="Identity">
          <setting>
            <aCSMoniker name="/Example/ExampleGroup/FTPServerWorker/FTP2Azure.MaxIdleSeconds" />
          </setting>
        </map>
        <map name="MapFTPServerWorker:FTP2Azure.Mode" kind="Identity">
          <setting>
            <aCSMoniker name="/Example/ExampleGroup/FTPServerWorker/FTP2Azure.Mode" />
          </setting>
        </map>
        <map name="MapFTPServerWorker:FTP2Azure.QueueNotification" kind="Identity">
          <setting>
            <aCSMoniker name="/Example/ExampleGroup/FTPServerWorker/FTP2Azure.QueueNotification" />
          </setting>
        </map>
        <map name="MapFTPServerWorker:FTP2Azure.StorageAccount" kind="Identity">
          <setting>
            <aCSMoniker name="/Example/ExampleGroup/FTPServerWorker/FTP2Azure.StorageAccount" />
          </setting>
        </map>
        <map name="MapFTPServerWorker:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/Example/ExampleGroup/FTPServerWorker/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapFTPServerWorkerInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/Example/ExampleGroup/FTPServerWorkerInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="FTPServerWorker" generation="1" functional="0" release="0" software="E:\opensauce\ftp2azure\Example\Example\csx\Debug\roles\FTPServerWorker" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="1792" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="FTP2Azure.Command" protocol="tcp" portRanges="9003" />
              <inPort name="FTP2Azure.Passive" protocol="tcp" portRanges="9002" />
            </componentports>
            <settings>
              <aCS name="FTP2Azure.ConnectionEncoding" defaultValue="" />
              <aCS name="FTP2Azure.FtpAccount" defaultValue="" />
              <aCS name="FTP2Azure.FtpServerHost" defaultValue="" />
              <aCS name="FTP2Azure.MaxClients" defaultValue="" />
              <aCS name="FTP2Azure.MaxIdleSeconds" defaultValue="" />
              <aCS name="FTP2Azure.Mode" defaultValue="" />
              <aCS name="FTP2Azure.QueueNotification" defaultValue="" />
              <aCS name="FTP2Azure.StorageAccount" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;FTPServerWorker&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;FTPServerWorker&quot;&gt;&lt;e name=&quot;FTP2Azure.Command&quot; /&gt;&lt;e name=&quot;FTP2Azure.Passive&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/Example/ExampleGroup/FTPServerWorkerInstances" />
            <sCSPolicyUpdateDomainMoniker name="/Example/ExampleGroup/FTPServerWorkerUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/Example/ExampleGroup/FTPServerWorkerFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyUpdateDomain name="FTPServerWorkerUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyFaultDomain name="FTPServerWorkerFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="FTPServerWorkerInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="45245e1d-4060-47aa-a9b6-02d3008058b5" ref="Microsoft.RedDog.Contract\ServiceContract\ExampleContract@ServiceDefinition">
      <interfacereferences>
        <interfaceReference Id="e6e6d2fc-7e8a-4639-a1f5-1a656dc5aecf" ref="Microsoft.RedDog.Contract\Interface\FTPServerWorker:FTP2Azure.Command@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/Example/ExampleGroup/FTPServerWorker:FTP2Azure.Command" />
          </inPort>
        </interfaceReference>
        <interfaceReference Id="6d1d6b41-756a-43c8-a8fd-1b34bf5d2b6f" ref="Microsoft.RedDog.Contract\Interface\FTPServerWorker:FTP2Azure.Passive@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/Example/ExampleGroup/FTPServerWorker:FTP2Azure.Passive" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>