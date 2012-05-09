using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Callisto.Data;
using Callisto.Data.SQLite;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Callisto.TestApp;
using System.Threading.Tasks;
using System.Text;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Callisto.TestApp.SamplePages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SqliteSample : Page
    {
        public SqliteSample()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        public void HandleCreateTableAsync(object sender, RoutedEventArgs e)
        {
            try
            {
                // run...
                var conn = GetConnection();
                conn.CreateTableAsync<Customer>().ContinueWith(async (tResult) =>
                {
                    await this.ShowAlertAsync("CreateTableAsync called OK. Number of objects: " + tResult.Result.ToString());

                }).ChainFailureHandler(this);
            }
            catch (Exception ex)
            {
                this.ShowAlertAsync(ex);
            }
        }

        public void HandleTableAsync(object sender, RoutedEventArgs e)
        {
            try
            {
                // run...
                var conn = GetConnection();
                conn.TableAsync<Customer>().ContinueWith(async (tResult) =>
                {
                    await this.ShowAlertAsync("TableAsync called OK. Number of rows: " + tResult.Result.Count());

                }).ChainFailureHandler(this);
            }
            catch (Exception ex)
            {
                this.ShowAlertAsync(ex);
            }
        }

        public void HandleInsertAsync(object sender, RoutedEventArgs e)
        {
            try
            {
                // run...
                Customer customer = new Customer()
                {
                    FirstName = "Foo",
                    LastName = "Bar",
                    Email = "foobar@mbrit.com"
                };

                var conn = GetConnection();
                conn.InsertAsync(customer).ContinueWith(async (tResult) =>
                 {
                     await this.ShowAlertAsync("Insert called OK. New ID: " + tResult.Result.ToString());

                 }).ChainFailureHandler(this);
            }
            catch (Exception ex)
            {
                this.ShowAlertAsync(ex);
            }
        }

        public void HandleQueryAsync(object sender, RoutedEventArgs e)
        {
            try
            {
                // run...
                throw new NotImplementedException("This operation has not been implemented.");
            }
            catch (Exception ex)
            {
                this.ShowAlertAsync(ex);
            }
        }

        public void HandleMultiple1(object sender, RoutedEventArgs e)
        {
            try
            {
                // run...
                var conn = this.GetConnection();
                conn.CreateTableAsync<Customer>().ContinueWith(async (r) =>
                {
                    // go...
                    Customer customer = new Customer("foo", "bar", "1");
                    await customer.InsertAsync(conn);
                    int result1 = customer.CustomerId;

                    // go...
                    customer = new Customer("foo", "bar", "2");
                    await customer.InsertAsync(conn);
                    int result2 = customer.CustomerId;

                    // go...
                    customer = new Customer("foo", "bar", "3");
                    await customer.InsertAsync(conn);
                    int result3 = customer.CustomerId;

                    // get all...
                    var all = await conn.TableAsync<Customer>();
                    await this.ShowAlertAsync(string.Format("Results: {0}, {1}, {2}, count: {3}", result1, result2, result3, all.Count()));

                });
            }
            catch (Exception ex)
            {
                this.ShowAlertAsync(ex);
            }
        }

        public void HandleMultiple2(object sender, RoutedEventArgs e)
        {
            try
            {
                // run...
                var conn = this.GetConnection();
                conn.CreateTableAsync<Customer>().ContinueWith(async (r) =>
                {
                    // go...
                    List<Customer> customers = new List<Customer>();
                    List<Task> tasks = new List<Task>();
                    for (int index = 0; index < 100; index++)
                    {
                        Customer customer = new Customer("foo", "bar", index.ToString());
                        customers.Add(customer);

                        // run...
                        tasks.Add(customer.InsertAsync(conn));
                    }

                    // walk...
                    Task.WaitAll(tasks.ToArray());

                    // get all...
                    var all = await conn.TableAsync<Customer>();
                  
                    // show...
                    StringBuilder builder = new StringBuilder();
                    builder.Append("Count: ");
                    builder.Append(all.Count());
                    builder.Append("\r\n--> ");
                    for (int index = 0; index < customers.Count; index++)
                    {
                        if (index > 0)
                            builder.Append(", ");
                        builder.Append(customers[index].CustomerId);
                    }
                    await this.ShowAlertAsync(builder.ToString());

                });
            }
            catch (Exception ex)
            {
                this.ShowAlertAsync(ex);
            }
        }

        private SQLiteAsyncConnection GetConnection()
        {
            // creates a database in the app's private storage...
            SQLiteAsyncConnection conn = new SQLiteAsyncConnection("foo.db", SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create |
                SQLiteOpenFlags.ReadWrite);

            return conn;
        }
    }
}
