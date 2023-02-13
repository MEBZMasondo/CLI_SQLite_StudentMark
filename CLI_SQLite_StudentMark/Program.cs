using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;


namespace CLI_SQLite_StudentMark
{
    class Program
    {
        
        SQLiteConnection connection; // Connection to the DB File

        static void Main(string[] args)
        {
            Program p = new Program();
        }

        public Program()
        {
            createNewDB();
            connectToDB();
            createTable();

            Boolean isContinue = true;
            while (isContinue)
            {
                int result = prompt();
                if (result == -2)
                    break;
                if (result == -1)
                {
                    Console.WriteLine("Unexpected response : PLEASE TRY AGAIN \n");
                    continue;
                }
                if (result == 1)
                {
                    printDataDesc();
                    continue;
                }
                if (result == 2)
                {
                    addNewRecords();
                    continue;
                }
                if (result == 3)
                {
                    updateRecord();
                    continue;
                }
                if (result == 4)
                {
                    deleteRecord();
                    continue;
                }
            }

            Console.WriteLine("PROGRAM EXECUTION ENDED, PRESS ENTER TO EXIT");
            Console.ReadLine();
        }

        int prompt()
        {
            try
            {
                Console.WriteLine("Enter your option number ");
                Console.WriteLine("[1] Display Data");
                Console.WriteLine("[2] Insert new Data Record");
                Console.WriteLine("[3] Update Record");
                Console.WriteLine("[4] Delete Record");
                Console.WriteLine("[9] Exit Program");

                int response;
                response = Convert.ToInt32(Console.ReadLine());

                switch (response)
                {
                    case 1:
                        return 1;
                    case 2:
                        return 2;
                    case 3:
                        return 3;
                    case 4:
                        return 4;
                    case 9:
                        return -2;
                    default:
                        return -1;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("ERROR: PROBLEM SELECTING OPERATION");
                return -1;
            }
        }

        void createNewDB() // Creates an empty SQLite DB file
        {
            try
            {
                SQLiteConnection.CreateFile("Database.sqlite");
            }
            catch (Exception exc)
            {
                Console.WriteLine("ERROR : CREATING DATABASE\n" + exc.Message);
            }
        }


        void connectToDB()  // Creates a connection with our SQLite DB file.
        {
            try
            {
                connection = new SQLiteConnection("Data Source=Database.sqlite; Version=3");
                connection.Open();
            }
            catch (Exception exc)
            {
                Console.WriteLine("ERROR : CONNECTION TO DATABASE\n" + exc.Message);
            }
        }

        void createTable()
        {
            try
            {
                string sql = "CREATE TABLE MarksTable (ID INTEGER PRIMARY KEY, name varchar(50), mark int)";
                SQLiteCommand command = new SQLiteCommand(sql, connection);
                command.ExecuteNonQuery();
            }
            catch (Exception exc)
            {
                Console.WriteLine("ERROR : CREATING TABLE\n" + exc.Message);
            }
        }

        void insertRecord(String name, int mark)
        {
            string sql = "INSERT INTO MarksTable (name, mark) values (' " + name + "', " + mark + ")";
            SQLiteCommand command = new SQLiteCommand(sql, connection);
            command.ExecuteNonQuery();
        }

        void addNewRecords()
        {
            Console.WriteLine("ADDING A NEW RECORD");
            Boolean isAdd = true;

            while (isAdd)
            {
                Console.WriteLine("Enter The Student name :");
                String name = Console.ReadLine();
                Console.WriteLine("Enter The Student mark :");
                int mark = Convert.ToInt32(Console.ReadLine());

                insertRecord(name, mark);

                Console.WriteLine("Add another record ? [y/n]");
                char input = Console.ReadLine()[0];
                Boolean isAsk = true;

                while (isAsk == true)
                {
                    if (input == 'y' || input == 'Y')
                    {
                        isAsk = true;
                        break;
                    }
                    if (input == 'n' || input == 'N')
                    {
                        isAsk = false;
                        isAdd = false;
                        break;
                    }
                    else
                    {
                        Console.WriteLine("PLEASE CHECK YOUR RESPONSE AND TRY AGAIN");
                    }
                }

            }

        }

        void updateRecord()
        {
            Console.WriteLine("Please Enter the ID to update :");
            int id = Convert.ToInt32(Console.ReadLine());

            // Check if record ID is within range
            int RowCount = 0;
            try
            {
                SQLiteCommand cmd = new SQLiteCommand(connection);

                cmd.CommandText = "SELECT COUNT(ID) FROM MarksTable";
                cmd.CommandType = CommandType.Text;
                RowCount = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR : VERIFYING ID RANGE\n" + ex.Message);
            }


            if (id < 0 || id > RowCount)
            {
                Console.WriteLine("The ID provided is not in the range expected, PLEASE TRY AGAIN");
                return;
            }

            Console.WriteLine("ARE YOU SURE YOU WANT TO UPDATE RECORD : " + id);
            char response = Console.ReadLine()[0];

            if (response == 'y' || response == 'Y')
            {
                try
                {
                    Console.WriteLine("Enter the new name :");
                    String name = Console.ReadLine();
                    Console.WriteLine("Enter new mark");
                    int mark = Convert.ToInt32(Console.ReadLine());

                    string sql = "UPDATE MarksTable SET name = '" + name + "', mark ='" + mark + "' WHERE ID = " + id + "";
                    SQLiteCommand command = new SQLiteCommand(sql, connection);
                    command.ExecuteNonQuery();
                    Console.WriteLine("RECORD ID: " + id + " Has Been Successfully updated");
                }
                catch (Exception exc)
                {
                    Console.WriteLine("ERROR : UPDATING RECORD\n" + exc.Message);
                }
            }
            if (response == 'n' || response == 'N')
            {
                Console.WriteLine("DELETE ARBORTED");
            }
            else
            {
                Console.WriteLine("DELETE NOT PROCESS, CHECK YOUR CONFIMATION AND TRY AGAIN");
            }


        }

        void deleteRecord()
        {
            Console.WriteLine("Please Enter the ID to delete :");
            int id = Convert.ToInt32(Console.ReadLine());

            // Check if record ID is within range
            int RowCount = 0;
            try
            {
                SQLiteCommand cmd = new SQLiteCommand(connection);

                cmd.CommandText = "SELECT COUNT(ID) FROM MarksTable";
                cmd.CommandType = CommandType.Text;
                RowCount = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR : VERIFYING ID RANGE\n" + ex.Message);
            }


            if (id < 0 || id > RowCount)
            {
                Console.WriteLine("The ID provided is not in the range expected, PLEASE TRY AGAIN");
                return;
            }

            Console.WriteLine("ARE YOU SURE YOU WANT TO DELETE RECORD : " + id);
            char response = Console.ReadLine()[0];

            if (response == 'y' || response == 'Y')
            {
                try
                {
                    string sql = "DELETE FROM " + "MarksTable" + " WHERE ID = '" + id + "'";
                    SQLiteCommand command = new SQLiteCommand(sql, connection);
                    command.ExecuteNonQuery();
                    Console.WriteLine("RECORD ID: " + id + " Has Been Successfully deleted");
                }
                catch (Exception exc)
                {
                    Console.WriteLine("ERROR : DELETING RECORD\n" + exc.Message);
                }
            }
            if (response == 'n' || response == 'N')
            {
                Console.WriteLine("DELETE ARBORTED");
            }
            else
            {
                Console.WriteLine("DELETE NOT PROCESS, CHECK YOUR CONFIMATION AND TRY AGAIN");
            }


        }


        void printDataDesc() // TODO : if no records, display message
        {
            Console.WriteLine("-----------------------------------------------------------------------");
            Console.WriteLine("                     STUDENTS MARKS TABLE DATA ");
            Console.WriteLine("-----------------------------------------------------------------------");
            try
            {
                string sql = "SELECT * FROM MarksTable ORDER BY mark DESC";
                SQLiteCommand command = new SQLiteCommand(sql, connection);
                SQLiteDataReader reader = command.ExecuteReader();
                Console.WriteLine("{0, -10} {1, -20} {2, -1}", "ID", "NAME", "MARK");
                Console.WriteLine("-----------------------------------------------------------------------");
                while (reader.Read())
                    Console.WriteLine("{0, -10} {1, -20} {2, -10}", reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2));
                //Console.WriteLine("Student ID: " + reader["ID"] + "\tName: " + reader["name"] + "\t Mark: " + reader["mark"]);
                Console.WriteLine("END OF RECORD, PRESS ENTER TO CONTINUE");
                Console.ReadLine();
            }
            catch (Exception exc)
            {
                Console.WriteLine("ERROR : DISPLAYING DATABASE TABLE DATA\n" + exc.Message);
            }
        }
    }
}
