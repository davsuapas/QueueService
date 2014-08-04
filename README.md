QueueService 
============

Example of processes execution service with RabbitMQ and. NET 

Documentation
-------------

In spanish: http://ejecucion-procesos-rabbitmq-dotnet.blogspot.com.es/2014/07/parte-1-introduccion-y-objetivos.html

Goals
-----

The main purpose of the processes service is create a basis for executing business logic that is available 24 hours a day. 

Key Features:

     - The service must be independent of the host. 
     - It should be easily configurable through a configuration xml file. 
     - The architecture should be modular to replace easily different parts of the service. 
     - Must be multi-connection queues to different servers. 
     - Business processes must be activated by messages from a queue server. 
     - It should be able to run different business processes independent of each other. 
     - Business processes should be independent of the service processes. As such, it should be automatically discovered by the service. 
     - The service must efficiently manage messages without overloading the server queues. 
     - Processes must run efficiently exploiting parallel cores of the machine. 
     - Scalability must be not only vertically but also horizontally. 
     - The service must be fault tolerant. 
     - The service must possess characteristics of orderly shutdown. 

Requirements development environment
------------------------------------

It has created a development environment based on the Windows 8 operating system and the following tools are installed:

     - Microsoft Visual Studio 2010. 
     - Managed Extensibility Framework (MEF): Is a composition layer for .NET that improves the flexibility, maintainability and testability of large applications. MEF can be used for third-party plugin extensibility, or it can bring the benefits of a loosely-coupled plugin-like architecture to regular applications.
     - RabbitMQ 3.2.4: Robust messaging for applications
     
