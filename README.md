__AdoProject__
---
This program is designed to work with data in MSSQL databases located on the server.
<p float="left">
<img src="Image\program\MainWindow.png" alt="MSSQL Connected" width="80"/>
<img src="Image\program\DB.png" alt="MSSQL Connected" width="200"/>
</p>

__Program work__
---
__1.Connection__
In order to connect to the MS SQL database through the "AdoProject" application, "Windows Authentication" is used. Therefore, Login and Password are not required.
<p float="left">
<img src="Image\MS_SQL_connected.png" alt="MSSQL Connected" width="150"/>
<img src="Image\Application_connected.png" alt="Application Connected" width="150"/>
</p>

__2 Features__
__2.1 Server__
The server and its name must contain the prefix "\SQLEXPRESS".
__2.2 DataBase__
In order for the program to work correctly with the Database, it is necessary that the tables have an identifier with the "Identity" property at position 0.
