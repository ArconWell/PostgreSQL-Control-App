# PostgreSQL-Control-App
GUI app which allows to start, stop and restart PostgreSQL Server without using CMD or Services

To configure the app you need to check your PostgreSQL service name using `Win+R -> "services.msc"`. After you should change variable `"ServiceName"` value in the file `"Form1.cs"` with the service name you checked before. Then recompile the app and everything is done.
