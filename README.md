QueueService
============

Servicio de processos con RabbitMQ y .NET


Objetivos
---------

El objetivo principal del servicio de procesos de alto rendimiento consiste en un procesador de lógica de negocio desatendido que se encuentre disponible 24 horas al día.

Características principales:
- El servicio debe ser independiente del host donde va a ser alojado.
- Debe ser fácilmente configurable a través de un fichero xml de configuración.
- La arquitectura debe ser modular para sustituir fácilmente diferentes partes del servicio.
- Debe ser multi-conexión a diferentes servidores de colas.
- Los procesos de negocio deben ser activados por mensajes de un servidor de colas.
- Se deben poder ejecutar diferentes procesos de negocio independientes unos de otros.
- Los procesos de negocio deben ser independientes del servicio de procesos. De tal forma, que deben ser descubiertos automáticamente por el servicio.
- El servicio debe gestionar eficientemente los mensajes sin sobrecargar el servidor de colas.
- Los procesos deben ejecutarse de forma paralela aprovechando eficientemente los núcleos de la máquina.
- La escalabilidad no solo deber ser vertical sino también horizontal.
- El servicio debe ser tolerante a fallos.
- El servicio debe poseer características de cierre ordenado.


Requisitos del entorno de desarrollo
------------------------------------

Se ha creado un entorno de desarrollo basado en el sistema operativo Windows 8 y se han instalado las siguientes herramientas:
- Microsoft Visual Studio 2010 Premium: Microsoft Visual Studio es un entorno de desarrollo integrado para crear soluciones para todo el porfolio de sistemas Windows; soluciones web, servicios web, Windows forms, etc. El entorno incluye editor de código, intelliSense, refactorización de código, depurador, diseñador de arquitecturas y de clases y se encuentra integrado con Microsoft Team System que es un entorno para el desarrollo de soluciones en equipo.
- RabbitMQ 3.2.4: Software de negociación de mensajes de código abierto.



