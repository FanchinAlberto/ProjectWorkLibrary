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

namespace ProjectWorkLibrary
{
    public partial class ManchesterLibrary : Form
    {
        public ManchesterLibrary()
        {
            InitializeComponent();
        }
        List<User> users;
        Dictionary<string, User> usersDict = new Dictionary<string, User>();
        List<Book> books;
        Dictionary<string, Book> booksDict = new Dictionary<string, Book>();
        Dictionary<string, Borrow> borrowDict = new Dictionary<string, Borrow>();
        List<Borrow> Loans = new List<Borrow>();
        public bool userTextChanged = false;
        public bool bookTextChanged = false;
        private void Form1_Load(object sender, EventArgs e)
        {
            users = JsonConvert.DeserializeObject<List<User>>(File.ReadAllText("users.json"));
            foreach(var user in users)
            {
                usersDict.Add(user.CF.ToString(), user);
            }
            viewUsers.DataSource = new BindingSource()
            {
                DataSource = new BindingList<User>(usersDict.Values.ToList())
            };

            books = JsonConvert.DeserializeObject<List<Book>>(File.ReadAllText("books.json"));
            foreach(var book in books)
            {
               booksDict.Add(book.ISBN, book);
            }
            viewBooks.DataSource = new BindingSource()
            {
                DataSource = new BindingList<Book>(booksDict.Values.ToList())
            };
            Loans = JsonConvert.DeserializeObject<List<Borrow>>(File.ReadAllText("loans.json"));
            BindLoans();

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
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            
            if (usersDict.ContainsKey(tbxUsername.Text) && usersDict[tbxUsername.Text.ToUpper()].Password == tbxPw.Text)
            {
                PrimaryTabControl.TabPages.Remove(tabPageLogin);
                if (usersDict[tbxUsername.Text.ToUpper()].Role == "user")
                {
                    PrimaryTabControl.TabPages.Remove(tabPageUserView);
                }
            }
            
        }
        private void btnBookFilter_Click(object sender, EventArgs e)
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

        private void btnAddUser_Click(object sender, EventArgs e)
        {
            User user = new User()
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
            usersDict.Add(tbxAddUserCF.Text, user);
            users.Add(user);
            File.WriteAllText("users.json", JsonConvert.SerializeObject(users));
        }

        private void btnRemoveUser_Click(object sender, EventArgs e)
        {
            usersDict.Remove(tbxRemoveUserCF.Text);
            foreach(var user in users)
            {
                if(user.CF == tbxRemoveUserCF.Text)
                {
                    users.Remove(user);
                }
            }
            File.WriteAllText("users.json", JsonConvert.SerializeObject(users));
        }

        private void btnModifyUserSearch_Click(object sender, EventArgs e)
        {
            DisplayFoundUsers(SearchUser());
        }

        private void DisplayFoundUsers(User u)
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
        private void textBoxUser_TextChanged(object sender, EventArgs e)
        {
            userTextChanged = true;
        }

        private void textBoxBook_TextChanged(object sender, EventArgs e)
        {
            bookTextChanged = true;
        }
        private void UpdateModifiedUsers(ref User u)
        {
            if (userTextChanged)
            {
                u.Name = tbxModifyUserName.Text;
                u.Surname = tbxModifyUserSurname.Text;
                u.Email = tbxModifyUserEmail.Text;
                u.Role = tbxModifyUserRole.Text;
                u.City = tbxModifyUserCity.Text;
                u.Password = tbxModifyUserPw.Text;
                u.CF = tbxModifyUserCF.Text;
                u.BirthDate = tbxModifyUserBirthDate.Text;
                File.WriteAllText("users.json", JsonConvert.SerializeObject(users));
            }
            
        }

        private void btnModifyUserConfirm_Click(object sender, EventArgs e)
        {
            User userFound = SearchUser();
            UpdateModifiedUsers(ref userFound);
        }

        private User SearchUser()
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

        private void btnFilterUsers_Click(object sender, EventArgs e)
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

        private void btnAddBook_Click(object sender, EventArgs e)
        {
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
            booksDict.Add(tbxAddBookISBN.Text, book);
            books.Add(book);
            File.WriteAllText("books.json", JsonConvert.SerializeObject(books));
        }

        private void btnRemoveBook_Click(object sender, EventArgs e)
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
        }

        private void btnSearchBook_Click(object sender, EventArgs e)
        {
            DisplayFoundBook(SearchBook());
        }

        private void DisplayFoundBook(Book b)
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

        private Book SearchBook()
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

        private void UpdateModifiedBooks(ref Book b)
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

        private void btnModifyBook_Click(object sender, EventArgs e)
        {
            Book b = SearchBook();
            UpdateModifiedBooks(ref b);
        }

        private void btnAddLoan_Click(object sender, EventArgs e)
        {
            Borrow br = new Borrow();
            
            foreach(var user in users)
            {
                if(user.CF == tbxAddLoanUserCF.Text)
                {
                    if(user.ActiveLoans > 0)
                    {
                        br.debtorUser = tbxAddLoanUserCF.Text;
                        user.ActiveLoans--;
                        br.startBorrow = dtpBeginLoan.Value;
                        br.endBorrow = dtpEndLoan.Value;
                        br.BorrowedBookISBN = tbxAddLoanBookISBN.Text;
                        br.LoanID = tbxLoanID.Text;
                        user.borrows.Add(br);
                        Loans.Add(br);
                        borrowDict.Add(tbxLoanID.Text, br);
                        BindLoans();
                    }
                    else
                    {
                        MessageBox.Show("L'utente ha già raggiunto il massimo di prestiti all'attivo");
                    }                    
                }
            }
            File.WriteAllText("loans.json", JsonConvert.SerializeObject(Loans));
        }

        private void BindLoans()
        {
            viewLoans.DataSource = null;
            viewLoans.DataSource = Loans;
        }

        private void btnFilterLoans_Click(object sender, EventArgs e)
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

        private void btnRemoveLoan_Click(object sender, EventArgs e)
        {
            Borrow borrow = SearchLoan();
            DeleteLoan(ref borrow);
            File.WriteAllText("loans.json", JsonConvert.SerializeObject(Loans));
        }

        private void DeleteLoan(ref Borrow loan)
        {
            Loans.Remove(loan);
        }

        private Borrow SearchLoan()
        {
            foreach(var loan in Loans)
            {
                if(loan.LoanID == tbxLoanID.Text)
                {
                    return loan;
                }
            }
            return null;
        }
    }
}
