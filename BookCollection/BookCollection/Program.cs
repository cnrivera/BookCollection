﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient; //tells program to use these specified classes to connect to an SQL database

namespace BookCollection
{
    class Program
    {
        static void Main(string[] args)
        {
            bool menuBreaker = false;
            Console.WriteLine("Welcome to the book collection, what would you like to do?");
            while (menuBreaker == false)
            {
                Console.WriteLine("----------\nMAIN MENU\n----------");
                Console.WriteLine("C:  Create a new book");
                Console.WriteLine("V:  View collection");
                Console.WriteLine("S:  Search collection");
                Console.WriteLine("U:  Update books");
                Console.WriteLine("D:  Delete books");
                Console.WriteLine("Q:  Quit the application");
                string response = Console.ReadLine().ToUpper();
                switch (response)
                {
                    case "V":
                        Book.viewAllBooks();
                        break;
                    case "C":
                        Book newBook = new Book();
                        Book.createNewBook(newBook);
                        Book.addNewBook(newBook);
                        break;
                    case "Q":
                        menuBreaker = true; //changes the menubreaker value, setting the while condition false and breaking the loop
                        break;
                    case "S":
                        Book.searchBooks();
                        break;
                    case "U":
                        Book.updateBooks();
                        break;
                    case "D":
                        Book.deleteBooks();
                        break;
                    default:
                        Console.WriteLine("Select one of the options:");
                        break;
                }
            }
        }
    }

    public class Database
    {
        public static SqlConnection bookCollectionConnection()
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = "Server=TERRY-PC; Database=BookCollection; Trusted_Connection=true";
            conn.Open();
            return conn;
        }

        public static void errorMessage(SqlCommand command, Book bookName)
        {
            try
            {
                command.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    bookName.errorMessages.Append("Index #" + i + "\n" +
                    "Message: " + ex.Errors[i].Message + "\n" +
                    "Error Number: " + ex.Errors[i].Number + "\n" +
                    "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                    "Source: " + ex.Errors[i].Source + "\n" +
                    "Procedure: " + ex.Errors[i].Procedure + "\n");
                }
                Console.WriteLine(bookName.errorMessages.ToString());
            }
            Console.ReadLine();
        }
    }

    public class Book
    {
        public string Title { get; set; }
        public string Series { get; set; }
        public string ISBN { get; set; }
        public string AuthorFirst { get; set; }
        public string AuthorLast { get; set; }
        public string Review { get; set; }
        public string genreField1 { get; set; } //Genre properties created so that each book can hold multiple genres. Individual properties are needed so that each genre can go into the genreList array as a seperate value. Putting them in all at once would lump them together into one value which would cause errors once transferred to the database
        public string genreField2 { get; set; }
        public string genreField3 { get; set; }
        public string genreField4 { get; set; }
        public string genreField5 { get; set; }
        public string genreField6 { get; set; }
        public string genreField7 { get; set; }
        public string genreField8 { get; set; }
        public string genreField9 { get; set; }
        public string genreField10 { get; set; }
        public string[] genreList { get; set; }

        public StringBuilder errorMessages = new StringBuilder();

        private static void displayBooks(SqlCommand command)
        {
            using (SqlDataReader reader = command.ExecuteReader())
            {
                Console.WriteLine("\nRESULTS:\n\n");
                while (reader.Read())
                {
                    Console.WriteLine("---------------\nTitle: {0}\nISBN: {1}\nAuthor: {2}\nSeries: {3}\nReview: {4}\nGenres: {5}\n---------------\n", reader[0], reader[1], reader[2], reader[3], reader[4], reader[5]);
                }
            }
        }

        private static void genreLoop(ref int i, int iValue, Book newBook, string genreField, ref string[] genreList) //selects one of the genre properties (up to 10), inserts the specified genre into it, extracts the value and places it into the genreList array
        {
            if (i == iValue)
            {
                var genreProperty = typeof(Book).GetProperty(genreField); //variable genre property fetches the actual property (indicated by the genreField parameter) of the referenced object (the newBook beging created). This was done because properties of objects cant be directly referenced as arguments into a functions
                genreProperty.SetValue(newBook, Console.ReadLine(),null); //since genreProperty is equivalent to the actual property (not to be confused with the value) changing it means changing the property
                Console.WriteLine("Done!");
                genreList[iValue] = genreProperty.GetValue(newBook, null).ToString(); //selects the value of the property and puts it in the array
            }
        }

        private static void genrePlaceholder(Book bookName, string action = null) //calls on the genreLoop function to place passed in genres into properties and then into the book genreList array. From there those genres can be put into the database or deleted.
        {
            string[] genreList = new string[10];
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("If you would like to "+action+" a genre type 'Y'. If no type 'N'");
                string continueLoop = Console.ReadLine();
                if (continueLoop.ToUpper() == "N" || i >= 10)
                {
                    break;
                }
                if (continueLoop.ToUpper() == "Y")
                {
                    Console.WriteLine("Please enter a genre for " + bookName.Title);
                    genreLoop(ref i, 0, bookName, "genreField1", ref genreList); //passes in the actual value of i, the specific value of i that refers to the desired stage through the loop, the newBook object being created and the name of the specific genre property to be set
                    genreLoop(ref i, 1, bookName, "genreField2", ref genreList);
                    genreLoop(ref i, 2, bookName, "genreField3", ref genreList);
                    genreLoop(ref i, 3, bookName, "genreField4", ref genreList);
                    genreLoop(ref i, 4, bookName, "genreField5", ref genreList);
                    genreLoop(ref i, 5, bookName, "genreField6", ref genreList);
                    genreLoop(ref i, 6, bookName, "genreField7", ref genreList);
                    genreLoop(ref i, 7, bookName, "genreField8", ref genreList);
                    genreLoop(ref i, 8, bookName, "genreField9", ref genreList);
                    genreLoop(ref i, 9, bookName, "genreField10", ref genreList);
                }
            }
            bookName.genreList = genreList;
        }

        public static Book createNewBook(Book newBook) //scope, association with object, return type, function name, parameter type and name
        {
            Console.WriteLine("What is the name of the book? This has to be unique.");
            newBook.Title = Console.ReadLine();
            Console.WriteLine("What series is this book part of? If not part of any, say 'None'.");
            newBook.Series = Console.ReadLine();
            Console.WriteLine("What is the ISBN? This has to be unique.");
            newBook.ISBN = Console.ReadLine();
            Console.WriteLine("Who is the author? First name is:");
            newBook.AuthorFirst = Console.ReadLine();
            Console.WriteLine("Who is the author? Last name is:");
            newBook.AuthorLast = Console.ReadLine();
            Console.WriteLine("What genre(s) does the book have? You can have up to 10.");
            genrePlaceholder(newBook, "add");
            Console.WriteLine("What is your review of the book? If you have not read it or do not want to write a review, say 'N/A'.");
            newBook.Review = Console.ReadLine();
            return newBook;
        }

        private static void addGenres(Book bookName)
        {
            SqlConnection conn = Database.bookCollectionConnection();
            SqlCommand insertGenre = new SqlCommand("spInsertGenre", conn);
            insertGenre.CommandType = System.Data.CommandType.StoredProcedure;
            SqlCommand insertB_IDAndG_ID = new SqlCommand("spInsertB_IDAndG_ID", conn);
            insertB_IDAndG_ID.CommandType = System.Data.CommandType.StoredProcedure;
            foreach (string genre in bookName.genreList)
            {
                if (genre == null)
                {
                    break;
                }
                insertGenre.Parameters.Clear(); //Used to clear out previous previous passed in values in the stored procedure. Without this, each time the loop runs more values get passed into the parameter of the stored procedure. The values get lumped together and increase with each pass through. Clearing the parameter ensures that only one value is sent into the parameter per pass through.
                insertGenre.Parameters.Add(new SqlParameter("@G", genre));
                insertGenre.ExecuteNonQuery();
                insertB_IDAndG_ID.Parameters.Clear();
                insertB_IDAndG_ID.Parameters.Add(new SqlParameter("@G", genre));
                insertB_IDAndG_ID.Parameters.Add(new SqlParameter("@T", bookName.Title));
                insertB_IDAndG_ID.ExecuteNonQuery();
            }
        }

        private static void deleteGenres(Book bookName, string recordName = null, string recordID = null)
        {
            SqlConnection conn = Database.bookCollectionConnection();
            SqlCommand deleteGenre = new SqlCommand("spdeleteGenre", conn);
            deleteGenre.CommandType = System.Data.CommandType.StoredProcedure;
            foreach (string genre in bookName.genreList)
            {
                if (genre == null)
                {
                    break;
                }
                deleteGenre.Parameters.Clear();
                deleteGenre.Parameters.Add(new SqlParameter("@G", genre));
                deleteGenre.Parameters.Add(new SqlParameter("@rName", recordName));
                deleteGenre.Parameters.Add(new SqlParameter("@rID", recordID));
                deleteGenre.ExecuteNonQuery();
            }
        }

        public static void addNewBook(Book newBook)
        {
            SqlConnection conn = Database.bookCollectionConnection();
            SqlCommand insertBookAndAuthor = new SqlCommand("spInsertBookAndAuthor @I, @T, @S, @R, @AF, @AL", conn);
            insertBookAndAuthor.Parameters.Add(new SqlParameter("I", newBook.ISBN));
            insertBookAndAuthor.Parameters.Add(new SqlParameter("T", newBook.Title));
            insertBookAndAuthor.Parameters.Add(new SqlParameter("S", newBook.Series));
            insertBookAndAuthor.Parameters.Add(new SqlParameter("R", newBook.Review));
            insertBookAndAuthor.Parameters.Add(new SqlParameter("AF", newBook.AuthorFirst));
            insertBookAndAuthor.Parameters.Add(new SqlParameter("AL", newBook.AuthorLast));
            Database.errorMessage(insertBookAndAuthor, newBook);
            /*try
            {
                insertBookAndAuthor.ExecuteNonQuery();
            }
            catch(SqlException ex)
            {
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    newBook.errorMessages.Append("Index #" + i + "\n" +
                        "Message: " + ex.Errors[i].Message + "\n" +
                        "Error Number: " + ex.Errors[i].Number + "\n" +
                        "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                        "Source: " + ex.Errors[i].Source + "\n" +
                        "Procedure: " + ex.Errors[i].Procedure + "\n");
                }
                Console.WriteLine(newBook.errorMessages.ToString());
            }
            Console.ReadLine();*/
            SqlCommand insertA_ID = new SqlCommand("spInsertA_ID @AF, @AL, @T", conn); 
            insertA_ID.Parameters.Add(new SqlParameter("AF", newBook.AuthorFirst));
            insertA_ID.Parameters.Add(new SqlParameter("AL", newBook.AuthorLast));
            insertA_ID.Parameters.Add(new SqlParameter("T", newBook.Title));
            insertA_ID.ExecuteNonQuery();
            addGenres(newBook);
        }

        public static void viewAllBooks()
        {
            SqlConnection conn = Database.bookCollectionConnection();
            SqlCommand viewAllBooks = new SqlCommand("spViewAllBooks", conn);
            displayBooks(viewAllBooks);
        }

        public static void searchBooks()
        {
            SqlConnection conn = Database.bookCollectionConnection();
            Console.WriteLine("Search by the following:");
            Console.WriteLine("T: Title");
            Console.WriteLine("A: Author");
            Console.WriteLine("I: ISBN");
            string response = Console.ReadLine().ToUpper();
            switch (response)
            {
                case "T":
                    Console.WriteLine("Enter the title you are searching for: ");
                    string title = Console.ReadLine();
                    SqlCommand searchByTitle = new SqlCommand("spSearchByTitle @T", conn);
                    searchByTitle.Parameters.Add(new SqlParameter("T", title));
                    displayBooks(searchByTitle);
                    break;
                case "A":
                    Console.WriteLine("Enter the first name of the author you are searching for: ");
                    string authorFirst = Console.ReadLine();
                    Console.WriteLine("Enter the last name of the author you are searching for: ");
                    string authorLast = Console.ReadLine();
                    SqlCommand searchByAuthor = new SqlCommand("spSearchByAuthor @AF, @AL", conn);
                    searchByAuthor.Parameters.Add(new SqlParameter("AF", authorFirst));
                    searchByAuthor.Parameters.Add(new SqlParameter("AL", authorLast));
                    displayBooks(searchByAuthor);
                    break;
                case "I":
                    Console.WriteLine("Enter the ISBN you are searching for: ");
                    string isbn = Console.ReadLine();
                    SqlCommand searchByISBN = new SqlCommand("spSearchByISBN @I", conn);
                    searchByISBN.Parameters.Add(new SqlParameter("I", isbn));
                    displayBooks(searchByISBN);
                    break;
                default:
                    Console.WriteLine("Select one of the options:");
                    break;
            }
        }

        private static void updateGenre(string fieldName, string recordID, string recordName, string storedProcedure, SqlConnection conn)
        {
            Book genreHolder = new Book();
            if (recordName == "Title")
            {
                genreHolder.Title = recordID;
            }
            else if (genreHolder.Title == null)
            {
                genreHolder.ISBN = recordID;
                using (SqlCommand retrieveTitle = new SqlCommand("spRetrieveTitle", conn)) //retrieves title of book if only ISBN is given
                {
                    retrieveTitle.CommandType = System.Data.CommandType.StoredProcedure;
                    SqlParameter T = new SqlParameter("T", genreHolder.Title);
                    T.Direction = System.Data.ParameterDirection.Output;
                    T.Size = 255;
                    retrieveTitle.Parameters.Add(T);
                    retrieveTitle.Parameters.Add(new SqlParameter("I", genreHolder.ISBN));
                    retrieveTitle.ExecuteNonQuery();
                    genreHolder.Title = retrieveTitle.Parameters["T"].Value.ToString();
                }
            }
            Console.WriteLine("If you want to add the genres type 'A', delete type 'D' or update type 'U':");
            var genreOptions = Console.ReadLine().ToUpper();
            switch (genreOptions)
            {
                case "A":
                    genrePlaceholder(genreHolder, "add");
                    addGenres(genreHolder);
                    break;
                case "D":
                    genrePlaceholder(genreHolder, "delete");
                    deleteGenres(genreHolder, recordName, recordID);
                    break;
                case "U":
                    SqlCommand deleteOldGenres = new SqlCommand("spDeleteOldGenres @rName, @rID", conn);
                    deleteOldGenres.Parameters.Add(new SqlParameter("rName", recordName));
                    deleteOldGenres.Parameters.Add(new SqlParameter("rID", recordID));
                    deleteOldGenres.ExecuteNonQuery();
                    genrePlaceholder(genreHolder, "update");
                    addGenres(genreHolder);
                    break;
            }
            Console.WriteLine("The " + fieldName + " are updated!");
        }

        private static void updateField(string fieldName, string recordID, string recordName, string storedProcedure = null, string tName = null, string fieldName1 = null, string fieldName2 = null)
        {
            SqlConnection conn = Database.bookCollectionConnection();
            Console.WriteLine("Would you like to update/delete the " + fieldName + "? Enter Y (yes) or N (no):");
            string recordN = recordName;
            string response = Console.ReadLine().ToUpper();
            switch (response) 
            {
                case "Y":
                    Console.WriteLine("If you want to delete a field (except genres) leave the field blank when prompted to type in a new value.");
                    SqlCommand updateField = new SqlCommand();
                    updateField.CommandType = System.Data.CommandType.StoredProcedure;
                    updateField.CommandText = storedProcedure;
                    {
                        if (fieldName1 != null && fieldName2 !=null) //this scenario is when there are two parts to a field name. Example - the author field is divided into FirstName and LastName in the database. To change it two new values need to be sent in as arguments into the function. This block deals with that scenario
                        {
                            Console.WriteLine("Enter the new " + fieldName1 + ":");
                            string valueNew1 = Console.ReadLine();
                            Console.WriteLine("Enter the new " + fieldName2 + ":");
                            string valueNew2 = Console.ReadLine();
                            updateField.Parameters.Add(new SqlParameter("VN1", valueNew1));
                            updateField.Parameters.Add(new SqlParameter("VN2", valueNew2));
                        }
                        if (fieldName == "Genre(s)")
                        {
                            updateGenre(fieldName, recordID, recordName, storedProcedure, conn);
                            break;
                        }
                        /*{
                            Book genreHolder = new Book();
                            if (recordN == "Title")
                            {
                                genreHolder.Title = recordID;
                            }
                            else if (genreHolder.Title == null)
                            {
                                genreHolder.ISBN = recordID;
                                using (SqlCommand retrieveTitle = new SqlCommand("spRetrieveTitle", conn)) //retrieves title of book if only ISBN is given
                                {
                                    retrieveTitle.CommandType = System.Data.CommandType.StoredProcedure;
                                    SqlParameter T = new SqlParameter("T", genreHolder.Title);
                                    T.Direction = System.Data.ParameterDirection.Output;
                                    T.Size = 255;
                                    retrieveTitle.Parameters.Add(T);
                                    retrieveTitle.Parameters.Add(new SqlParameter("I", genreHolder.ISBN));
                                    retrieveTitle.ExecuteNonQuery();
                                    genreHolder.Title = retrieveTitle.Parameters["T"].Value.ToString();
                                }
                            }
                            Console.WriteLine("If you want to add the genres type 'A', delete type 'D' or update type 'U':");
                            var genreOptions = Console.ReadLine().ToUpper();
                            switch (genreOptions)
                            {
                                case "A":
                                    genrePlaceholder(genreHolder, "add");
                                    addGenres(genreHolder);
                                    break;
                                case "D":
                                    genrePlaceholder(genreHolder, "delete");
                                    deleteGenres(genreHolder, recordN, recordID);
                                    break;
                                case "U":
                                    SqlCommand deleteOldGenres = new SqlCommand("spDeleteOldGenres @rName, @rID", conn);
                                    deleteOldGenres.Parameters.Add(new SqlParameter("rName", recordN));
                                    deleteOldGenres.Parameters.Add(new SqlParameter("rID", recordID));
                                    deleteOldGenres.ExecuteNonQuery();
                                    genrePlaceholder(genreHolder, "update");
                                    addGenres(genreHolder);
                                    break;
                            }
                            Console.WriteLine("The " + fieldName + " are updated!");
                            break;
                        }*/
                        else
                        {
                            Console.WriteLine("Enter the new " + fieldName + ":");
                            string valueNew = Console.ReadLine();
                            updateField.Parameters.Add(new SqlParameter("VN", valueNew));
                            updateField.Parameters.Add(new SqlParameter("rField", fieldName)); //field that needs to be updated
                        }
                        updateField.Parameters.Add(new SqlParameter("rID", recordID)); //identifier of record in database
                        updateField.Parameters.Add(new SqlParameter("rName", recordN)); //search criteria in database, title or isbn
                        updateField.Parameters.Add(new SqlParameter("tName", tName));//table that contains field to be updated
                        updateField.Connection = conn;
                        updateField.ExecuteNonQuery();
                        Console.WriteLine("The " +fieldName+ " is updated!");
                        break;
                    }
                case "N":
                    {
                        break;
                    }
                default:
                    {
                        Console.WriteLine("Please enter Y (yes) or N (No) for updating the " +fieldName+ ".");
                        break;
                    }
            }
        }

        private static void runUpdateFieldFunctions(string recordName)
        {
            Console.WriteLine("You can update or delete all the fields except the "+recordName+". Enter the "+recordName+" of the book you want to update:");
            string recordID = Console.ReadLine();
            string recordN = recordName;
            if (recordN == "ISBN")
            {
                updateField("Title", recordID, recordN, "spUpdateField", "Books");
            }
            else if (recordN == "Title")
            {
                updateField("ISBN", recordID, recordN, "spUpdateField", "Books");
            }
            updateField("Series", recordID, recordN, "spUpdateField", "Books");
            updateField("Author", recordID, recordN, "spUpdateAuthor", "first name", "last name");
            updateField("Review", recordID, recordN, "spUpdateField", "Books");
            updateField("Genre(s)", recordID, recordN);
        }

        public static void updateBooks() 
        {
            Console.WriteLine("Which book would you like to change? You can select by ISBN or title. Select I (ISBN) or T (Title) below:");
            string response = Console.ReadLine().ToUpper();
            switch (response)
            {
                case "I":
                    runUpdateFieldFunctions("ISBN");
                    break;
                case "T":
                    runUpdateFieldFunctions("Title");
                    break;
                default:
                    Console.WriteLine("Back to to the main menu!");
                    break;
            }
        }

        private static void deleteBook(string recordType)
        {
            SqlConnection conn = Database.bookCollectionConnection();
            Console.WriteLine("Enter the " + recordType + " of the book that you want to delete:");
            string recordID = Console.ReadLine();
            SqlCommand deleteBook = new SqlCommand("spDeleteBook @rID, @rType", conn);
            deleteBook.Parameters.Add(new SqlParameter("rID", recordID));
            deleteBook.Parameters.Add(new SqlParameter("rType", recordType));
            deleteBook.ExecuteNonQuery();
            Console.WriteLine("The book with the "+recordType+" of "+recordID+" has been deleted!");
        }

        public static void deleteBooks()
        {
            Console.WriteLine("Which book would you like to delete? You can select by ISBN or title. Select I (ISBN) or T (Title) below:");
            string response = Console.ReadLine().ToUpper();
            switch (response)
            {
                case "I":
                    deleteBook("ISBN");
                    break;
                case "T":
                    deleteBook("Title");
                    break;
                default:
                    Console.WriteLine("Back to to the main menu!");
                    break;
            }
        }
    }
}

      

