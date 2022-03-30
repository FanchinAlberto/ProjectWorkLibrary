using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Net;

namespace ProjectWorkLibrary
{
    public partial class ManchesterLibrary : Form
    {
        public ManchesterLibrary()
        {
            InitializeComponent();
        }
        List<User> users; //lista contenente tutti gli utenti
        Dictionary<string, User> usersDict = new Dictionary<string, User>(); //dictionary di utenti, la chiave è il codice fiscale, il value è l'oggetto User
        List<Book> books; //lista contenente i libri
        Dictionary<string, Book> booksDict = new Dictionary<string, Book>(); //dictionary di libri, la chiave è il codice ISBN, il value è l'oggetto Book
        Dictionary<string, Borrow> borrowDict = new Dictionary<string, Borrow>(); //dictionary di libri, la chiave è il codice del Loan, il value è l'oggetto Borrow
        List<Borrow> Loans = new List<Borrow>(); //lista contenente i prestiti
        List<Borrow> ExpiredLoans = new List<Borrow>();
        public bool userTextChanged = false;
        public bool bookTextChanged = false;
        DateTime checker = DateTime.Today;
        private void Form1_Load(object sender, EventArgs e)
        {
            users = JsonConvert.DeserializeObject<List<User>>(File.ReadAllText("users.json")); //Importo dal file json i dati per la lista di utenti
            foreach (var user in users) //inserisco gli utenti nella dictionary
            {
                usersDict.Add(user.CF.ToString(), user);
            }
            viewUsers.DataSource = new BindingSource() //mostro gli utenti nella DataGridView
            {
                DataSource = new BindingList<User>(usersDict.Values.ToList())
            };

            books = JsonConvert.DeserializeObject<List<Book>>(File.ReadAllText("books.json")); //stassa cosa degli user
            foreach (var book in books)
            {
                booksDict.Add(book.ISBN, book);
            }
            viewBooks.DataSource = new BindingSource()
            {
                DataSource = new BindingList<Book>(booksDict.Values.ToList())
            };
            Loans = JsonConvert.DeserializeObject<List<Borrow>>(File.ReadAllText("loans.json")); //Importo i prestiti dal file json
            BindLoans();

            //aggiungo alle textbox della modifica dei dati di utenti e libri un eventHandler personalizzato
            tbxModifyUserName.TextChanged += new System.EventHandler(this.textBoxUser_TextChanged);
            tbxModifyUserSurname.TextChanged += new System.EventHandler(this.textBoxUser_TextChanged);
            tbxModifyUserEmail.TextChanged += new System.EventHandler(this.textBoxUser_TextChanged);
            tbxModifyUserCity.TextChanged += new System.EventHandler(this.textBoxUser_TextChanged);
            tbxModifyUserPw.TextChanged += new System.EventHandler(this.textBoxUser_TextChanged);
            tbxModifyUserCF.TextChanged += new System.EventHandler(this.textBoxUser_TextChanged);
            tbxModifyUserBirthDate.TextChanged += new System.EventHandler(this.textBoxUser_TextChanged);
            tbxModifyUserRole.TextChanged += new System.EventHandler(this.textBoxUser_TextChanged);


            tbxModifyBookTitle.TextChanged += new System.EventHandler(this.textBoxBook_TextChanged);
            tbxModifyBookISBN.TextChanged += new System.EventHandler(this.textBoxBook_TextChanged);
            tbxModifyBookThumbnail.TextChanged += new System.EventHandler(this.textBoxBook_TextChanged);
            tbxModifyBookAuthors.TextChanged += new System.EventHandler(this.textBoxBook_TextChanged);
            rtbxModifyBookDescription.TextChanged += new System.EventHandler(this.textBoxBook_TextChanged);
            tbxModifyBookSubtitles.TextChanged += new System.EventHandler(this.textBoxBook_TextChanged);
            tbxModifyBookCategories.TextChanged += new System.EventHandler(this.textBoxBook_TextChanged);
            tbxModifyBookYear.TextChanged += new System.EventHandler(this.textBoxBook_TextChanged);
            tbxModifyBookAverageRating.TextChanged += new System.EventHandler(this.textBoxBook_TextChanged);
            tbxModifyBookRatingsCount.TextChanged += new System.EventHandler(this.textBoxBook_TextChanged);
            tbxModifyBookQuantity.TextChanged += new System.EventHandler(this.textBoxBook_TextChanged);
            tbxModifyBookPages.TextChanged += new System.EventHandler(this.textBoxBook_TextChanged);

            viewDashboard.DataSource = books; //aggiungo i valori alla dashboard

            //rimuovo le tab page, in modo che l'utente non possa accedervi prima del login
            PrimaryTabControl.TabPages.Remove(tabPageUserView);
            PrimaryTabControl.TabPages.Remove(tabViewBooks);
            PrimaryTabControl.TabPages.Remove(tabManageUsers);
            PrimaryTabControl.TabPages.Remove(tabManageBooks);
            PrimaryTabControl.TabPages.Remove(tabDashBoard);
            PrimaryTabControl.TabPages.Remove(tabLoans);
            PrimaryTabControl.TabPages.Remove(tabHistory);

            //Righe che servono per non mostrare le tendine in alto dei tabControl
            PrimaryTabControl.Appearance = TabAppearance.FlatButtons;
            PrimaryTabControl.ItemSize = new Size(0, 1);
            PrimaryTabControl.SizeMode = TabSizeMode.Fixed;

            UserManagementTabControl.Appearance = TabAppearance.FlatButtons;
            UserManagementTabControl.ItemSize = new Size(0, 1);
            UserManagementTabControl.SizeMode = TabSizeMode.Fixed;

            BookManagementTabControl.Appearance = TabAppearance.FlatButtons;
            BookManagementTabControl.ItemSize = new Size(0, 1);
            BookManagementTabControl.SizeMode = TabSizeMode.Fixed;

            ManageLoansTabControl.Appearance = TabAppearance.FlatButtons;
            ManageLoansTabControl.ItemSize = new Size(0, 1);
            ManageLoansTabControl.SizeMode = TabSizeMode.Fixed;


            CheckLoans(); //controllo i prestiti scaduti
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            //controllo che i dati inseriti corrispondano ad un utente reale
            if (usersDict.ContainsKey(tbxUsername.Text) && usersDict[tbxUsername.Text.ToUpper()].Password == tbxPw.Text)
            {
                PrimaryTabControl.TabPages.Remove(tabPageLogin); //rimuovo la page del login
                if (usersDict[tbxUsername.Text.ToUpper()].Role == "user") //l'utente è un user
                {
                    PrimaryTabControl.TabPages.Add(tabViewBooks); //gli mostro solo le tab dei libri e dello storico prestiti
                    PrimaryTabControl.TabPages.Add(tabHistory);

                    btnGoToBookManagement.Visible = false;
                    btnGoToUsersManagement.Visible = false;
                    btnGoToDashboard.Visible = false;
                    btnGoToLoans.Visible = false;
                    btnGoToUsers.Visible = false;
                    btnGoToHistory.Location = new Point(0, 268);
                }
                else if(usersDict[tbxUsername.Text.ToUpper()].Role == "admin") //l'utente è admin
                {
                    PrimaryTabControl.TabPages.Add(tabPageUserView); //mostro tutte le pagine
                    PrimaryTabControl.TabPages.Add(tabViewBooks);
                    PrimaryTabControl.TabPages.Add(tabManageUsers);
                    PrimaryTabControl.TabPages.Add(tabManageBooks);
                    PrimaryTabControl.TabPages.Add(tabDashBoard);
                    PrimaryTabControl.TabPages.Add(tabLoans);
                    PrimaryTabControl.TabPages.Add(tabHistory);
                }
            }
            
           

            User u = usersDict[tbxUsername.Text]; //controllo se l'utente ha diritto a un premio per i 10 libri
            if(u.borrows.Count == 10)
            {
                u.hasReward = true;
            }

            users.First(o => o == u).hasReward = u.hasReward;

            foreach(var b in Loans)
            {
                if(b.debtorUser == tbxUsername.Text)
                {
                    u.borrows.Add(b);
                }
            }
            BindBorrows(u);
        }
        private void btnBookFilter_Click(object sender, EventArgs e) //filtro i libri in base a ciò che l'user inserisce nelle textbox
        {
            var results = from b in books
                          where b.ISBN.ToUpper().Contains(tbxISBNFilter.Text.ToUpper())
                          && b.Title.ToUpper().Contains(tbxTitleFilter.Text.ToUpper())
                          && b.Authors.ToUpper().Contains(tbxAuthorsFilter.Text.ToUpper())
                          && b.Subtitle.ToUpper().Contains(tbxSubtitlesFilter.Text.ToUpper())
                          && b.Published_Year.ToUpper().Contains(tbxYearFilter.Text.ToUpper())
                          && b.Categories.ToUpper().Contains(tbxCategoriesFilter.Text.ToUpper())
                          && b.Qta.ToUpper().Contains(tbxQtaFilter.Text.ToUpper())
                          && b.Description.ToUpper().Contains(tbxDescriptionFilter.Text.ToUpper())
                          && b.Num_Pages.ToUpper().Contains(tbxPageFilter.Text.ToUpper())
                          && b.Average_Rating.ToUpper().Contains(tbxRatingFilter.Text.ToUpper())
                          select b;
            viewBooks.DataSource = null;

            viewBooks.DataSource = new BindingSource()
            {
                DataSource = new BindingList<Book>(results.ToList())

            };
        }

        private void btnAddUser_Click(object sender, EventArgs e) //Aggiunta di un user
        {
            if (tbxAddUserName.Text == "" || tbxAddUserSurname.Text == "" || tbxAddUserEmail.Text == "" || tbxAddUserRole.Text == "" || tbxAddUserCity.Text == "" || tbxAddUserCF.Text == "" || tbxAddUserPw.Text == "" || tbxAddUserBirthDate.Text == "" || tbxAddUserCF.Text.Length != 16)
            {//controllo che i dati inseriti siano validi
                MessageBox.Show("Inserire dei dati validi");
                return;
            }
            User user = new User() //creo il nuovo utente
            {
                Name = tbxAddUserName.Text,
                Surname = tbxAddUserSurname.Text,
                Email = tbxAddUserEmail.Text,
                Role = tbxAddUserRole.Text,
                City = tbxAddUserCity.Text,
                CF = tbxAddUserCF.Text,
                Password = tbxAddUserPw.Text,
                BirthDate = tbxAddUserBirthDate.Text
            };
            usersDict.Add(tbxAddUserCF.Text, user); //aggiungo l'utente nella lista e nel dictionary e riscrivo il json per aggiungerlo
            users.Add(user);
            File.WriteAllText("users.json", JsonConvert.SerializeObject(users));
            BindUsers(); //ricarico la datagridview degli user, in modo che il nuovo utente venga visualizzato
        }

        private void btnRemoveUser_Click(object sender, EventArgs e) //rimozione user
        {
            usersDict.Remove(tbxRemoveUserCF.Text); //rimuovo l'user la cui key corrisponde al codice fiscale inserito nella textbox
            foreach (var user in users)
            {
                if (user.CF == tbxRemoveUserCF.Text)
                {
                    users.Remove(user);
                }
            }
            File.WriteAllText("users.json", JsonConvert.SerializeObject(users)); //riscrivo il json e updato la datagridview
            BindUsers();
        }

        private void btnModifyUserSearch_Click(object sender, EventArgs e)
        {
            DisplayFoundUsers(SearchUser());
        }

        private void DisplayFoundUsers(User u) //mostro l'utente che ho cercato
        {
            tbxModifyUserName.Text = u.Name;
            tbxModifyUserSurname.Text = u.Surname;
            tbxModifyUserEmail.Text = u.Email;
            tbxModifyUserCity.Text = u.City;
            tbxModifyUserPw.Text = u.Password;
            tbxModifyUserCF.Text = u.CF;
            tbxModifyUserBirthDate.Text = u.BirthDate;
            tbxModifyUserRole.Text = u.Role;
        }
        private void textBoxUser_TextChanged(object sender, EventArgs e) //handler personalizzato
        {
            userTextChanged = true;
        }

        private void textBoxBook_TextChanged(object sender, EventArgs e) //handler personalizzato
        {
            bookTextChanged = true;
        }
        private void UpdateModifiedUsers(ref User u)
        {
            if (userTextChanged) //se il testo è stato cambiato modifico i dati dell'user
            {
                u.Name = tbxModifyUserName.Text;
                u.Surname = tbxModifyUserSurname.Text;
                u.Email = tbxModifyUserEmail.Text;
                u.Role = tbxModifyUserRole.Text;
                u.City = tbxModifyUserCity.Text;
                u.Password = tbxModifyUserPw.Text;
                u.CF = tbxModifyUserCF.Text;
                u.BirthDate = tbxModifyUserBirthDate.Text;
                File.WriteAllText("users.json", JsonConvert.SerializeObject(users)); //riscrivo il json
            }

        }

        private void btnModifyUserConfirm_Click(object sender, EventArgs e)
        {
            User userFound = SearchUser(); //cerco l'user
            UpdateModifiedUsers(ref userFound); //mostro i risultati
            BindUsers(); //aggiorno la datagridview
        }

        private User SearchUser()//funzione che cerca un user con il codice fiscale
        {
            foreach (var user in users)
            {
                if (user.CF == tbxModifyUserSearch.Text)
                {
                    return user;
                }
            }
            return null;
        }

        private void btnFilterUsers_Click(object sender, EventArgs e) //filtro gli user in base a cio che l'utente scrive nelle textbox
        {
            var results = from u in users
                          where u.Name.ToUpper().Contains(tbxNameFilter.Text.ToUpper())
                          && u.Surname.ToUpper().Contains(tbxSurnameFilter.Text.ToUpper())
                          && u.Email.ToUpper().Contains(tbxEmailFilter.Text.ToUpper())
                          && u.Role.ToUpper().Contains(tbxRoleFilter.Text.ToUpper())
                          && u.City.ToUpper().Contains(tbxCityFilter.Text.ToUpper())
                          && u.CF.ToUpper().Contains(tbxCFFilter.Text.ToUpper())
                          && u.Password.ToUpper().Contains(tbxPwFilter.Text.ToUpper())
                          && u.BirthDate.ToUpper().Contains(tbxBirthDateFilter.Text.ToUpper())
                          select u;

            viewUsers.DataSource = null;

            viewUsers.DataSource = new BindingSource()
            {
                DataSource = new BindingList<User>(results.ToList())

            };
        }

        private void btnAddBook_Click(object sender, EventArgs e)//aggiunta libro
        {
            if (tbxAddBookTitle.Text == "" || tbxAddBookISBN.Text == "" || tbxAddBookThumbnail.Text == "" || tbxAddBookAuthors.Text == "" || rtbxAddBookDescription.Text == "" || tbxAddBookSubtitles.Text == "" || tbxAddBookCategories.Text == "" || tbxAddBookYear.Text == "" || tbxAddBookAverageRating.Text == "" || tbxAddBookRatingsQuantity.Text == "" || tbxAddBookQuantity.Text == "" || tbxAddBookPages.Text == "")
            {//controllo che i dati siano validi
                MessageBox.Show("Inserire dei dati validi");
                return;
            }
            Book book = new Book()
            {
                Title = tbxAddBookTitle.Text,
                ISBN = tbxAddBookISBN.Text,
                Thumbnail = tbxAddBookThumbnail.Text,
                Authors = tbxAddBookAuthors.Text,
                Description = rtbxAddBookDescription.Text,
                Subtitle = tbxAddBookSubtitles.Text,
                Categories = tbxAddBookCategories.Text,
                Published_Year = tbxAddBookYear.Text,
                Average_Rating = tbxAddBookAverageRating.Text,
                Ratings_Count = tbxAddBookRatingsQuantity.Text,
                Qta = tbxAddBookQuantity.Text,
                Num_Pages = tbxAddBookPages.Text
            };
            booksDict.Add(tbxAddBookISBN.Text, book); //uguale a quanto fatto per l'aggiunta dell'user
            books.Add(book);
            File.WriteAllText("books.json", JsonConvert.SerializeObject(books));
            BindBooks();
        }

        private void btnRemoveBook_Click(object sender, EventArgs e) //uguale alla rimozione dell'user
        {
            booksDict.Remove(tbxRemoveBook.Text);
            foreach (var book in books)
            {
                if (book.ISBN == tbxRemoveBook.Text)
                {
                    books.Remove(book);
                }
            }
            File.WriteAllText("books.json", JsonConvert.SerializeObject(books));
            BindBooks();
        }

        private void btnSearchBook_Click(object sender, EventArgs e)
        {
            DisplayFoundBook(SearchBook());
        }

        private void DisplayFoundBook(Book b) //uguale al display dell'user
        {
            tbxModifyBookTitle.Text = b.Title;
            tbxModifyBookISBN.Text = b.ISBN;
            tbxModifyBookAuthors.Text = b.Authors;
            tbxModifyBookCategories.Text = b.Categories;
            tbxModifyBookThumbnail.Text = b.Thumbnail;
            tbxModifyBookSubtitles.Text = b.Subtitle;
            rtbxModifyBookDescription.Text = b.Description;
            tbxModifyBookYear.Text = b.Published_Year;
            tbxModifyBookAverageRating.Text = b.Average_Rating;
            tbxModifyBookRatingsCount.Text = b.Ratings_Count;
            tbxModifyBookPages.Text = b.Num_Pages;
            tbxModifyBookQuantity.Text = b.Qta;
        }

        private Book SearchBook() //uguale al searche dell'user, cosice ISBN per trovare il libro
        {
            foreach (var book in books)
            {
                if (book.ISBN == tbxSearchBook.Text)
                {
                    return book;
                }
            }
            return null;
        }

        private void UpdateModifiedBooks(ref Book b) //uguale alla modifica dell'user
        {
            if (bookTextChanged)
            {
                b.ISBN = tbxModifyBookISBN.Text;
                b.Title = tbxModifyBookTitle.Text;
                b.Thumbnail = tbxModifyBookThumbnail.Text;
                b.Authors = tbxModifyBookAuthors.Text;
                b.Categories = tbxModifyBookCategories.Text;
                b.Description = rtbxModifyBookDescription.Text;
                b.Average_Rating = tbxModifyBookAverageRating.Text;
                b.Ratings_Count = tbxModifyBookRatingsCount.Text;
                b.Num_Pages = tbxModifyBookPages.Text;
                b.Subtitle = tbxModifyBookSubtitles.Text;
                b.Published_Year = tbxModifyBookYear.Text;
                b.Qta = tbxModifyBookQuantity.Text;
                File.WriteAllText("books.json", JsonConvert.SerializeObject(books));
            }
        }

        private void btnModifyBook_Click(object sender, EventArgs e)//uguale modifica user
        {
            Book b = SearchBook();
            UpdateModifiedBooks(ref b);
            BindBooks();
        }

        private void btnAddLoan_Click(object sender, EventArgs e) //aggiunta di un prestito
        {
            Borrow br = new Borrow();
            Book b = booksDict[tbxAddLoanBookISBN.Text];
            int quantity = Int32.Parse(b.Qta);
            foreach (var user in users) //scorro users
            {
                if (user.CF == tbxAddLoanUserCF.Text) //cerco l'user selezionato
                {
                    if (user.ActiveLoans > 0 && quantity > 0) //controllo che l'user possa prendere in prestito il libro
                    {
                        br.debtorUser = tbxAddLoanUserCF.Text;
                        user.ActiveLoans--;
                        double difference = (dtpEndLoan.Value - dtpBeginLoan.Value).TotalDays;
                        if (difference < 30 && difference >= 0) //controllo che il prestito non superi i 30 giorni
                        {
                            br.startBorrow = dtpBeginLoan.Value; //creo un nuovo prestito 
                            br.endBorrow = dtpEndLoan.Value;
                            br.BorrowedBookISBN = tbxAddLoanBookISBN.Text;
                            br.LoanID = tbxLoanID.Text;
                            user.borrows.Add(br);
                            if (Loans == null)
                            {
                                Loans = new List<Borrow>();
                                Loans.Add(br);
                            }
                            else
                            {
                                Loans.Add(br);
                            }
                            usersDict[br.debtorUser].borrows.Add(br);
                            quantity--;
                            b.Qta = quantity.ToString();
                            borrowDict.Add(tbxLoanID.Text, br);
                            BindLoans();
                            books.First(o => o == b).Qta = b.Qta;
                            BindBooks();
                            BindBorrows(usersDict[br.debtorUser]);
                        }
                        else
                        {
                            MessageBox.Show("A loan can last maximum 30 days, please select a valid date");
                        }

                    }
                    else if (user.ActiveLoans == 0)
                    {
                        MessageBox.Show("L'utente ha già raggiunto il massimo di prestiti all'attivo");
                    }
                    else if (quantity == 0)
                    {
                        MessageBox.Show("Le copie del libro sono terminate");
                    }
                }
            }
            File.WriteAllText("loans.json", JsonConvert.SerializeObject(Loans)); //aggiorno il file json con i prestiti
        }

        private void BindLoans()
        {
            viewLoans.DataSource = null;
            List<Borrow> allLoans = Loans.Concat(ExpiredLoans).ToList();
            viewLoans.DataSource = allLoans;
        }

        private void btnFilterLoans_Click(object sender, EventArgs e) //filtro i prestiti
        {
            var results = from loan in Loans
                          where loan.BorrowedBookISBN.Contains(tbxLoanFilterISBN.Text)
                          && loan.debtorUser.Contains(tbxLoanFilterCF.Text)
                          && loan.startBorrow == dtpBeginLoan.Value
                          && loan.endBorrow == dtpEndLoan.Value
                          && loan.LoanID.Contains(tbxLoanID.Text)
                          select loan;
            viewLoans.DataSource = null;
            viewLoans.DataSource = new BindingSource()
            {
                DataSource = new BindingList<Borrow>(results.ToList())
            };
        }

        private void btnRemoveLoan_Click(object sender, EventArgs e) //rimozione di un prestito
        {
            Borrow borrow = SearchLoan();
            User u = usersDict[borrow.debtorUser]; //aggiorno i prestiti disponibili per l'utente
            int loanCount = u.ActiveLoans;
            loanCount++;
            u.ActiveLoans = loanCount;
            users.First(o => o == u).ActiveLoans = u.ActiveLoans;
            DeleteLoan(ref borrow); //rimuovo il prestito
            File.WriteAllText("loans.json", JsonConvert.SerializeObject(Loans));
            BindLoans();
            Book b = booksDict[tbxRemoveLoanBookISBN.Text]; //aggiorno la quantità del libro
            int quantity = Int32.Parse(b.Qta);
            quantity++;
            b.Qta = quantity.ToString();
            books.First(o => o == b).Qta = b.Qta;
            RateBook();

            
        }

        private void DeleteLoan(ref Borrow loan) //elimino il prestito
        {
            Loans.Remove(loan);
            ExpiredLoans.Add(loan);
        }

        private Borrow SearchLoan() //cerco il prestito
        {
            foreach (var loan in Loans)
            {
                if (loan.LoanID == tbxRemoveLoanID.Text)
                {
                    return loan;
                }
            }
            return null;
        }

        private void BindBooks()
        {
            viewBooks.DataSource = null;
            viewBooks.DataSource = books;
            File.WriteAllText("books.json", JsonConvert.SerializeObject(books));
        }
        private void BindUsers()
        {
            viewUsers.DataSource = null;
            viewUsers.DataSource = users;
            File.WriteAllText("users.json", JsonConvert.SerializeObject(users));
        }

        private void RateBook() //valutazione libri
        {
            Book b = booksDict[tbxRemoveLoanBookISBN.Text];
            int newRating = (Int32)nudRating.Value;
            double averageRating = Double.Parse(b.Average_Rating);
            averageRating /= 100;
            int ratingCount = Int32.Parse(b.Ratings_Count);
            averageRating = (newRating + averageRating * ratingCount) / ++ratingCount;
            b.Average_Rating = averageRating.ToString();
            b.Ratings_Count = ratingCount.ToString();
            books.First(o => o == b).Average_Rating = b.Average_Rating;
            books.First(o => o == b).Ratings_Count = b.Ratings_Count;
            BindBooks();
        }

        private void viewBooks_SelectionChanged(object sender, EventArgs e) //mostro la thumbnail nell'apposita picturebox
        {
            var select = viewBooks.CurrentRow;

            if (select == null) return;
            string url = select.Cells["Thumbnail"].Value.ToString();
            ptbShowThumbnail.Load(url);
        }

        private void CheckLoans() //controllo che i prestiti non siano scaduti
        {
            foreach (var loan in Loans)
            {
                double difference = (loan.endBorrow - checker).TotalDays;
                if (difference == 0)
                {
                    SendMail(loan.debtorUser); //mando una mail se il prestito è scaduto
                }
            }
        }

        private void SendMail(string CF) //invio una mail all'utente che non restituisce il libro
        {
            string from, to, pw, messageBody;
            MailMessage message = new MailMessage();
            to = usersDict[CF].Email;
            from = "projectworklibrary@gmail.com";
            pw = "ProjectWorkLibrary2022!";
            messageBody = "Hi, this email i a reminder." +
                $"You didn't return the book we borrowed to you, pls come and return the book as soon as possible." +
                "Not returning the book will insert you in the black list.";
            message.To.Add(to);
            message.From = new MailAddress(from);
            message.Body = messageBody;
            message.Subject = "Book Borrow Not Returned";
            message.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient("smtp.gmail.com");
            smtp.EnableSsl = true;
            smtp.Port = 587;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Credentials = new NetworkCredential(from, pw);

            try
            {
                smtp.Send(message);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Email non inviata, controllare l'indirizzo email dell'utente {usersDict[CF].GetNameSurname()}");
                return;
            }
        }

        private void btnFilterDashboard_Click(object sender, EventArgs e) //filtro la dashboard
        {
            switch (cmbChoseFilterDashboard.SelectedIndex)
            {
                case 0:
                    var resultsOrderedDescendingRating = from b in booksDict
                                                         orderby Double.Parse(b.Value.Average_Rating) descending
                                                         select b.Value;

                    List<Book> orderByDescendingRating = resultsOrderedDescendingRating.ToList();
                    viewDashboard.DataSource = null;
                    viewDashboard.DataSource = orderByDescendingRating;
                    break;
                case 1:
                    var resultsOrderedIncreasingRating = from b in booksDict
                                                         orderby Double.Parse(b.Value.Average_Rating) ascending
                                                         select b.Value;

                    List<Book> orderByIncreasingRating = resultsOrderedIncreasingRating.ToList();
                    viewDashboard.DataSource = null;
                    viewDashboard.DataSource = orderByIncreasingRating;
                    break;
                case 2:
                    var resultsOrderedDescendingRatingCount = from b in booksDict
                                                              orderby Double.Parse(b.Value.Average_Rating) descending
                                                              select b.Value;

                    List<Book> orderByDescendingRatingCount = resultsOrderedDescendingRatingCount.ToList();
                    viewDashboard.DataSource = null;
                    viewDashboard.DataSource = orderByDescendingRatingCount;
                    break;
                case 3:
                    var resultsOrderedIncreasingRatingCount = from b in booksDict
                                                              orderby Double.Parse(b.Value.Average_Rating) ascending
                                                              select b.Value;

                    List<Book> orderByIncreasingRatingCount = resultsOrderedIncreasingRatingCount.ToList();
                    viewDashboard.DataSource = null;
                    viewDashboard.DataSource = orderByIncreasingRatingCount;
                    break;
            }
        }

        private void exportGridToPdf(string filename) //esportazione della datagridview in formato pdf
        {
            //creo la pdftable, che verrà poi inserita nel pdf
            BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1250, BaseFont.EMBEDDED);
            PdfPTable pdftable = new PdfPTable(viewDashboard.Columns.Count);
            pdftable.DefaultCell.Padding = 3;
            pdftable.WidthPercentage = 100;
            pdftable.HorizontalAlignment = Element.ALIGN_LEFT;
            pdftable.DefaultCell.BorderWidth = 1;

            //Aggiunta degli headers
            iTextSharp.text.Font text = new iTextSharp.text.Font(bf, 10, iTextSharp.text.Font.NORMAL);
            foreach(DataGridViewColumn column in viewDashboard.Columns)
            {
                PdfPCell cell = new PdfPCell(new Phrase(column.HeaderText, text));
                cell.BackgroundColor = new iTextSharp.text.BaseColor(240, 240, 240);
                pdftable.AddCell(cell);
            }

            //Aggiunta delle righe contenenti i dati
            foreach(DataGridViewRow row in viewDashboard.Rows)
            {
                foreach(DataGridViewCell cell in row.Cells)
                {
                    pdftable.AddCell(new Phrase(cell.Value.ToString(), text));
                }
            }
            //chiedo dove salvare il file
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = filename;
            saveFileDialog.DefaultExt = ".pdf";
            if(saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using(FileStream stream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                {
                    Document pdfDocument = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                    PdfWriter.GetInstance(pdfDocument, stream);
                    pdfDocument.Open();
                    pdfDocument.Add(pdftable);
                    pdfDocument.Close();
                    stream.Close();
                }
            }
        }
        private void btnGetFileDownload_Click(object sender, EventArgs e)
        {
            string filename = $"Results"+ cmbChoseFilterDashboard.SelectedItem.ToString(); //genero il nome del file
            exportGridToPdf(filename); //creo e salvo il pdf
        }

        private void btnGoToAddUser_Click(object sender, EventArgs e)
        {
            UserManagementTabControl.SelectedIndex = 0;
        }

        private void btnGoToRemoveUser_Click(object sender, EventArgs e)
        {
            UserManagementTabControl.SelectedIndex = 1;
        }

        private void btnGoToEditUser_Click(object sender, EventArgs e)
        {
            UserManagementTabControl.SelectedIndex = 2;
        }

        private void btnGoToAddBook_Click(object sender, EventArgs e)
        {
            BookManagementTabControl.SelectedIndex = 0;
        }

        private void btnGoToRemoveBook_Click(object sender, EventArgs e)
        {
            BookManagementTabControl.SelectedIndex = 1;
        }

        private void btnGoToEditBook_Click(object sender, EventArgs e)
        {
            BookManagementTabControl.SelectedIndex = 2;
        }

        private void btnGoToBooks_Click(object sender, EventArgs e)
        {
            PrimaryTabControl.SelectedTab = tabViewBooks;
        }

        private void btnGoToAddLoan_Click(object sender, EventArgs e)
        {
            ManageLoansTabControl.SelectedIndex = 0;
        }

        private void btnGoToViewLoan_Click(object sender, EventArgs e)
        {
            ManageLoansTabControl.SelectedIndex = 1;
        }

        private void btnGoToRemoveLoan_Click(object sender, EventArgs e)
        {
            ManageLoansTabControl.SelectedIndex = 2;
        }

        private void btnGoToUsers_Click(object sender, EventArgs e)
        {
            PrimaryTabControl.SelectedTab = tabPageUserView;
        }

        private void btnGoToUsersManagement_Click(object sender, EventArgs e)
        {
            PrimaryTabControl.SelectedTab = tabManageUsers;
        }

        private void btnGoToBookManagement_Click(object sender, EventArgs e)
        {
            PrimaryTabControl.SelectedTab = tabManageBooks;
        }

        private void btnGoToLoans_Click(object sender, EventArgs e)
        {
            PrimaryTabControl.SelectedTab = tabLoans;
        }

        private void btnGoToDashboard_Click(object sender, EventArgs e)
        {
            PrimaryTabControl.SelectedTab = tabDashBoard;
        }

        private void btnGoToHistory_Click(object sender, EventArgs e)
        {
            PrimaryTabControl.SelectedTab = tabHistory;
        }

        private void BindBorrows(User u)
        {
            viewHistory.DataSource = null;
            viewHistory.DataSource = u.borrows;
        }

    }
    
}
