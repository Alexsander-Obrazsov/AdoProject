__AdoProject__
---
__Description__
This program is designed to work with data in MSSQL databases located on the server.
<p float="left">
<img src="Image\program\MainWindow.png" alt="MSSQL Connected" width="80"/>
<img src="Image\program\DB.png" alt="MSSQL Connected" width="200"/>
</p>

___

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

___

__Guide__
---
__1 Connection__
To connect to the database, enter the name of your MSSQL server in the field named "Server" and click on the "Connect to the Server" button. Select the desired database and click on the "Connect to the DataBase" button.
<img src="Image\program\MainWindow.png" alt="MSSQL Connected" width="150"/>
__2 Working with the database__
In the open window, a field will appear in which data from the selected table will be displayed, a window with a list of tables in the selected database, buttons for working with data and a button to exit the database entry window.
__2.1 Selected Table__
In order to work with the desired table, select it in the window with a list of tables, which is located on the top right.
__2.2 Add line__
To add a row to the table, click on the "Add line" button. A window will appear in which you need to enter data into the corresponding columns of the text fields. The first text field cannot be added because it is an identifier.
__2.3 Edit line__
To change a row in the table, click on the "Edit line" button. A window will appear in which you need to change the data in the corresponding columns of the text fields. The first text field cannot be edited because it is an identifier.
__2.4 Delete line__
To delete a row in the table, select the desired row and click on the "Delete line" button. The selected row will be deleted automatically.
__2.5 Clear table__
To clear the entire table, click on the "Clear table" button and click "Yes" in the window that appears. The selected table will be deleted.
__3 Exit__
To exit the database editing window, click on the "Back" button. The database editing window will close and the database connection window will open. If you need to exit the application completely, click on the "Exit" button in the database connection window.