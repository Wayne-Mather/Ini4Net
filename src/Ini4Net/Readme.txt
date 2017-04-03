INI4NET
-------

Copyright (c) 2008-2011 Wayne Mather
All Rights Reserved

------------------------------------------------

Even with today's powerhouses of computers and structured data sets, I have found myself relying on the ability to process
configuration files in various applications that I have needed to write.

For this purpose I have created the Ini4Net project so that everyone using .NET can have the abilty to utilise plain
text files for configuration options.

I prefer plain text as they are easier to read and manipulate than XML documents.

The following example shows a simple configuration file in the INI format:

#
# Ini File Example
#
# v1.0
#
[database]
server = (local)
database = thisDb
user = userA
pwd = Sup3rS3cr3t!				

[logging]
database = log
fallback = c:\temp\application.log

[calendar]
type = monthly
start = 1/1/2011

You can see that identifying critical sections is easy and modification is just as easy