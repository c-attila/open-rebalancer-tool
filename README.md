## Open Rebalancing Tool
This is a small console application created for myself to ease the process of rebalancing my cryptocurrency portfolio on Binance.

**Build**

Use Visual Studio if you are familiar with it:

 - Simply clone the repository or download the project through GitHub
   and import it into Visual Studio.
  - After importing it, you should be
   able to build and run it from the 'Build' and 'Debug' menus.

You can also use MSBuild which is an easier tool to build projects created in VS without installing VS:

 - Download the 'Build Tools for Visual Studio 2019' from
   [visualstudio.microsoft.com/downloads](https://visualstudio.microsoft.com/downloads/)
   and install it.
  - After the installation, you can start the tool by typing
   '*Developer PowerShell for VS 2019*' into the search bar on taskbar
   (Windows 10). After this, navigate to the location of the project and
   enter *MSBuild '.\OpenRebalancerTool.csproj' /p:OutDir=.\out* . You
   should find the project in the 'out' folder. Read more
   [here](https://docs.microsoft.com/en-us/visualstudio/msbuild/msbuild?view=vs-2019).

**Use**

To use the application, you need 3 things:
 - Your API key and secret key that are provided by Binance when you create an API key (Account > Settings > API Management menu).
 - The tracked assets (coins and fiats) that you want to rebalance with the tool and their desired distribution (in percentage).
 - The primary and secondary assets that the tool will use to buy and sell, ie. if your primary asset is EUR and your secondary asset is BTC, the tool will try to buy for BTC and sell for Euro (if those are not possible, it will try the other one).

Start the application with the following command line arguments: **[apiKey] [secretKey] [primary asset] [seconday asset] [asset1] [target1] [asset2] [target2]**.. or enter them after you started the application.
For example: **xyzapikeyxyz xyzsecretkeyxyz EUR BTC BTC 50 ETH 30 EUR 20**
After you start the application, it will show the current and the current balances, the target distribution of your portfolio and their total in BTC. After you press a key, it will start the rebalancing. Before each order, it will test it if it can be executed and if so, ask you if you want to proceed with the given order. You have to choose y or n for every asset that passes the filters.

*P.S: I take no responsibility for any loss you may have using this application.*
