# Coding Tracker
![List Session](https://i.imgur.com/WusUsN8.gif)  
A redux of the [Habit Tracker](https://github.com/sitauros/Demo_Habit_Logger) project that now tracks coding session datetimes instead of companies.  
Record your session's *start* and *end* times via the instructions below to calculate the duration in human-friendly format.  
New features include classes to hold database data, a SQL subquery to combine statements, an XML config file for project constants, and higher-resolution gifs.  

  * [Add a Session](#add-a-sesssion)
  * [Update Session Info](#update-session-info)
  * [Delete a Session](#delete-a-session)
  * [Useful Links](#useful-links)

## Add a Session
![Add Session](https://i.imgur.com/Lqh6zKB.gif)
* Enter your session's start and end times, and be sure to include any leading zeroes.
* Should you format your datetime incorrectly, choose an end time prior to your start time, or input a non-existent leap day, the gremlins inside the app will sternly warn you about your gaffe.
* Be sure to memorize the session ID for future use.

## Update Session Info
![Update Session](https://i.imgur.com/ZG12iBE.gif)
* This option will allow you to alter your session's start or end time, but you cannot alter them both in tandem.
* Similar rules regarding adding a session apply here for datetimes. 

## Delete a Session
![Delete Session](https://i.imgur.com/22dvWEN.gif)
* This option will allow you delete a session from the database.
* Simply recite the unwanted session's ID to play the Wrath of God.

## Useful Links
* [C# Academy](https://www.thecsharpacademy.com/project/13) - Lists the original project requirements
* [ConsoleTableExt](https://github.com/minhhungit/ConsoleTableExt) - Used to format the tables in C# console applications
* [Dev.to](https://dev.to/appwrite/this-is-why-you-should-use-cursor-pagination-4nh5) - Article comparing cursor and offset-based pagination
* [System Configuration Manager](https://www.nuget.org/packages/System.Configuration.ConfigurationManager/) - Workaround for adding .NET assembly references
