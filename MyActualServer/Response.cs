using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace MyActualServer
{
    class Response
    {
        public Response()
        {
        }

        public void responseGenerator(HttpListenerContext context, string url, string method, string body)
        {
            //outer if/else chain checks to see which resource the client is requesting and sends the appropriate document           
            if (url == "http://localhost:8000/")
            {
                //checks to see if the client sent a post request and included data in the body of the request
                //if so, the appropriate handler will update the .txt file, so if the server is stopped or shutdown, data will be persisted
                if (method == "POST" && body.Length > 0)
                {
                    //the data is formatted so that each entry is a line in a text file
                    //in each line the column values are separated by commas
                    //these steps separate the body of the request into the appropriate columns and creates an array, each member of which is an entry in the data text file 
                    string[] changeRequest = body.Split(',');
                    string data = "";
                    string[] fileData = File.ReadAllLines(@"data.txt");
                    Console.WriteLine("Information received:");

                    //creates a correctly formatted row out of the body data
                    for (int i = 0; i < (changeRequest.Length - 1); i++)
                    {
                        data += ", ";
                        Console.WriteLine(changeRequest[i]);
                        data += changeRequest[i] + " ";
                    }

                    //gets the type of request the client sent, if they're adding, modifying, or deleting a row
                    string mode = changeRequest[changeRequest.Length - 1];

                    if (mode == "add")
                    {
                        //gets the index number of the last line in the current data file, adds an index number one greater than that to the line being added to the file
                        string[] lastLine = fileData[fileData.Length - 1].Split(',');
                        string number = lastLine[1].Trim();
                        int numberLine;
                        int.TryParse(number.Substring(1), out numberLine);

                        string index = ", %" + (numberLine + 1).ToString() + " " + data;

                        File.AppendAllText(@"data.txt", "\r\n" + index);
                    }
                    else if (mode == "modify")
                    {
                        //searches the file for a row with the same index number as the client data
                        //replaces the data in that row with the data the client sent
                        string newFile = "";
                        string[] inputLine = data.Split(',');
                        for (int i = 0; i < fileData.Length; i++)
                        {
                            string[] currentLine = fileData[i].Split(',');
                            if (currentLine[1] == inputLine[1])
                            {
                                fileData[i] = data;
                            }
                            if (i != (fileData.Length - 1))
                            {
                                newFile += fileData[i] + "\r\n";
                            }
                            else
                            {
                                newFile += fileData[i];
                            }
                        }
                        File.WriteAllText(@"data.txt", newFile);
                    }
                    else
                    {
                        //this searches the file in the same manner as the modify handler
                        //once the line is found, the entry is removed. Every line read after has its index number decreased by one
                        string newFile = "";
                        string[] inputLine = data.Split(',');
                        bool deleted = false;
                        for (int i = 0; i < fileData.Length; i++)
                        {
                            string[] currentLine = fileData[i].Split(',');
                            if (i != (fileData.Length - 1))
                            {
                                fileData[i] += "\r\n";
                            }                           
                            if (currentLine[1] == inputLine[1])
                            {
                                fileData[i] = "";
                                deleted = true;
                            }
                            else
                            {
                                if (deleted)
                                {
                                    string[] split = fileData[i].Split(',');
                                    string number = split[1].Trim();
                                    int newNumber;
                                    int.TryParse(number.Substring(1), out newNumber);
                                    newNumber -= 1;
                                    string newIndex = ", %" + newNumber;
                                    fileData[i] = newIndex + fileData[i].Substring(fileData[i].IndexOf('%') + 2);
                                }
                                newFile += fileData[i];
                            }
                        }
                        if (newFile.Substring(newFile.Length - 2) == "\r\n")
                        {
                            newFile = newFile.Remove(newFile.Length - 2);
                        }
                        File.WriteAllText(@"data.txt", newFile);
                    }                    
                }
                sendResponse(File.ReadAllText(@"mainPage.html"), context, "html");
            }
            //continuation of checking to see which resource was requested
            else if (url == "http://localhost:8000/mystyle.css")
            {
                sendResponse(File.ReadAllText(@"mystyle.css"), context, "css");
            }
            else if (url == "http://localhost:8000/JavaScript.js")
            {
                sendResponse(File.ReadAllText(@"JavaScript.js"), context, "js");
            }
            else
            {
                sendResponse(File.ReadAllText(@"mainPage.html"), context, "html");
            }
            
            
        }
        public void sendResponse(string responseString, HttpListenerContext context, string type)
        {
            if (type == "html")
            {
                //gets the HTML file and the data file.
                //formats the text in the data file into however many html rows are needed
                //uses a placeholder in the html file to append the formatted data table into the html
                string dataString = File.ReadAllText(@"data.txt");
                string[] rowSplit = dataString.Split(',');
                string html = "";
                int j = 1;
                int k = 1;

                for (int i = 1; i < rowSplit.Length; i++)
                {
                    if (j != 1)
                    {
                        if (j == 2)
                        {
                            html += "<tr> <td>" + k.ToString() + "</td>";
                        }
                        html += "<td>" + rowSplit[i] + "</td>";
                        if (j == 4)
                        {
                            html += "</tr>";
                            j = 0;
                            k++;
                        }
                    }                    
                    j++;
                }
                responseString = string.Format(responseString, html);
            }

            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            HttpListenerResponse response = context.Response;
            
            //sets the appropriate MIME type for the server response
            if (type == "css")
            {
                response.ContentType = "text/css";
            }
            else if (type == "js")
            {
                response.ContentType = "application/javascript";
            }

          /* this was for sending the icon the browser was requesting, but resulted in the page loading as a picture of the icon. Even without it, the icon appears to load fine on tabs.
             else if (type == "ico")
            {
                response.ContentType ="image/webp";
            } */

            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            // You must close the output stream.
            output.Close();
        }
    }
}
