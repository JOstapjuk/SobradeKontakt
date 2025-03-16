namespace SobradeKontakt
{
    public partial class MainPage : ContentPage
    {
        List<Contact> contacts;
        TableView tableView;
        Entry messageEntry;
        Random random = new Random();
        private Contact selectedContact = null;

        // Form entries
        Entry nameEntry;
        Entry phoneEntry;
        Entry emailEntry;
        Entry descriptionEntry;
        bool isFormVisible = false;

        public MainPage()
        {
            Title = "Sõbrade kontaktandmed";

            contacts = new List<Contact>
            {
                new Contact { Name = "Mati", Phone = "+3721234567", Email = "mati@example.com", Description = "Lapsepõlve sõber", Image = "dotnet_bot.png" },
                new Contact { Name = "Mari", Phone = "+3727654321", Email = "mari@example.com", Description = "Kolleeg tööl", Image = "dotnet_bot.png" }
            };

            messageEntry = new Entry
            {
                Placeholder = "Sisesta sõnum...",
                HorizontalOptions = LayoutOptions.Fill,
                BackgroundColor = Colors.White,
                TextColor = Colors.Black
            };

            tableView = new TableView
            {
                Intent = TableIntent.Form,
                Root = new TableRoot(),
                BackgroundColor = Colors.White
            };

            PopulateTable();

            // Custom message button
            Button customMessageButton = new Button
            {
                Text = "Saada oma sõnum",
                BackgroundColor = Color.FromArgb("#2196F3"), // Blue
                TextColor = Colors.White,
                VerticalOptions = LayoutOptions.Start,
                HeightRequest = 50,
                Margin = new Thickness(0, 10, 0, 5)
            };
            customMessageButton.Clicked += SendCustomMessage;

            // Random holiday greeting button
            Button randomHolidayButton = new Button
            {
                Text = "Saada juhuslik pühadetervitus",
                BackgroundColor = Color.FromArgb("#FF9800"), // Orange
                TextColor = Colors.White,
                VerticalOptions = LayoutOptions.Start,
                HeightRequest = 50,
                Margin = new Thickness(0, 5, 0, 10)
            };
            randomHolidayButton.Clicked += SendRandomHolidayGreeting;

            // Buttons horizontal layout
            HorizontalStackLayout buttonsLayout = new HorizontalStackLayout
            {
                Spacing = 10,
                HorizontalOptions = LayoutOptions.Fill,
                Children = { customMessageButton, randomHolidayButton }
            };

            // Button to show/hide contact form
            Button addContactButton = new Button
            {
                Text = "Lisa uus kontakt",
                BackgroundColor = Color.FromArgb("#4CAF50"), // Green
                TextColor = Colors.White,
                VerticalOptions = LayoutOptions.Start,
                HeightRequest = 50,
                Margin = new Thickness(0, 5, 0, 10)
            };
            addContactButton.Clicked += ToggleContactForm;

            // Create contact form
            nameEntry = new Entry { Placeholder = "Nimi", BackgroundColor = Colors.White, TextColor = Colors.Black };
            phoneEntry = new Entry { Placeholder = "Telefon", BackgroundColor = Colors.White, TextColor = Colors.Black, Keyboard = Keyboard.Telephone };
            emailEntry = new Entry { Placeholder = "Email", BackgroundColor = Colors.White, TextColor = Colors.Black, Keyboard = Keyboard.Email };
            descriptionEntry = new Entry { Placeholder = "Kirjeldus", BackgroundColor = Colors.White, TextColor = Colors.Black };

            Button saveContactButton = new Button
            {
                Text = "Salvesta kontakt",
                BackgroundColor = Color.FromArgb("#673AB7"), // Purple
                TextColor = Colors.White,
                HeightRequest = 50
            };
            saveContactButton.Clicked += SaveNewContact;

            VerticalStackLayout contactForm = new VerticalStackLayout
            {
                Spacing = 10,
                Padding = new Thickness(10),
                BackgroundColor = Color.FromArgb("#E0E0E0"),
                IsVisible = false, // Initially hidden
                Children = {
                    new Label { Text = "Lisa uus kontakt", FontSize = 18, FontAttributes = FontAttributes.Bold, TextColor = Colors.Black },
                    nameEntry,
                    phoneEntry,
                    emailEntry,
                    descriptionEntry,
                    saveContactButton
                }
            };

            StackLayout layout = new StackLayout
            {
                Padding = new Thickness(10),
                Spacing = 15,
                BackgroundColor = Color.FromArgb("#F5F5F5"),
                Children = { tableView, messageEntry, buttonsLayout, addContactButton, contactForm }
            };

            ScrollView scrollView = new ScrollView
            {
                Content = layout
            };

            Content = scrollView;
        }

        private void PopulateTable()
        {
            var tableSection = new TableSection("Kontaktid");

            foreach (var contact in contacts)
            {
                var photo = new Image
                {
                    Source = contact.Image,
                    HeightRequest = 40,
                    WidthRequest = 40,
                    Aspect = Aspect.AspectFill
                };

                // Gesture to change photo
                var photoTapGesture = new TapGestureRecognizer();
                photoTapGesture.Tapped += async (s, e) => await ChangePhoto(contact);
                photo.GestureRecognizers.Add(photoTapGesture);

                var nameLabel = new Label
                {
                    Text = contact.Name,
                    FontSize = 18,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Colors.Black
                };

                var callButton = new Button
                {
                    Text = "Helista",
                    BackgroundColor = Color.FromArgb("#4CAF50"),
                    TextColor = Colors.White,
                    HeightRequest = 40,
                    WidthRequest = 100
                };
                callButton.Clicked += (sender, e) => MakeCall(contact.Phone);

                // Create the layout for the contact row
                var layout = new HorizontalStackLayout
                {
                    Spacing = 10,
                    Children = { photo, nameLabel, callButton }
                };

                // Create a Gesture Recognizer for the whole row to select contact
                var rowTapGesture = new TapGestureRecognizer();
                rowTapGesture.Tapped += (s, e) => SelectContact(contact);

                // Add the row tap gesture to the row layout
                layout.GestureRecognizers.Add(rowTapGesture);

                // Add the layout to the table section
                var viewCell = new ViewCell { View = layout };
                tableSection.Add(viewCell);
            }

            tableView.Root.Clear();
            tableView.Root.Add(tableSection);
        }


        private void SelectContact(Contact contact)
        {
            selectedContact = contact;

            // Optionally, give feedback to the user that the contact has been selected
            DisplayAlert("Kontakt valitud", $"Valisid kontakti {contact.Name}", "OK");
        }

        private async void MakeCall(string phoneNumber)
        {
            if (await Launcher.CanOpenAsync($"tel:{phoneNumber}"))
            {
                await Launcher.OpenAsync($"tel:{phoneNumber}");
            }
            else
            {
                await DisplayAlert("Viga", "Helistamine ei ole toetatud", "OK");
            }
        }

        private async void SendSms(string phoneNumber)
        {
            if (selectedContact == null)
            {
                await DisplayAlert("Viga", "Palun vali kontakt", "OK");
                return;
            }

            string message = messageEntry.Text ?? "Tere tulemast! Saadan sõnumi";
            if (!string.IsNullOrWhiteSpace(phoneNumber) && Sms.Default.IsComposeSupported)
            {
                SmsMessage sms = new SmsMessage(message, new List<string> { phoneNumber });
                await Sms.Default.ComposeAsync(sms);
            }
            else
            {
                await DisplayAlert("Viga", "SMS saatmine ei ole toetatud", "OK");
            }
        }

        private async void SendEmail(string email)
        {
            if (selectedContact == null)
            {
                await DisplayAlert("Viga", "Palun vali kontakt", "OK");
                return;
            }

            string message = messageEntry.Text ?? "Tere tulemast! Saadan emaili";
            if (!string.IsNullOrWhiteSpace(email) && Email.Default.IsComposeSupported)
            {
                EmailMessage e_mail = new EmailMessage
                {
                    Subject = "Tervitus",
                    Body = message,
                    BodyFormat = EmailBodyFormat.PlainText,
                    To = new List<string> { email }
                };

                await Email.Default.ComposeAsync(e_mail);
            }
            else
            {
                await DisplayAlert("Viga", "E-kirja saatmine ei ole toetatud", "OK");
            }
        }

        private async Task ChangePhoto(Contact contact)
        {
            var cameraStatus = await Permissions.RequestAsync<Permissions.Camera>();

            if (cameraStatus != PermissionStatus.Granted)
            {
                await DisplayAlert("Luba puudub", "Kaamera kasutamiseks on vaja luba.", "OK");
                return;
            }

            try
            {
                var photo = await MediaPicker.CapturePhotoAsync();
                if (photo != null)
                {
                    string localPath = Path.Combine(FileSystem.AppDataDirectory, $"{contact.Name}_photo.jpg");

                    using (var stream = await photo.OpenReadAsync())
                    using (var newFile = File.OpenWrite(localPath))
                    {
                        await stream.CopyToAsync(newFile);
                    }

                    contact.Image = localPath;
                    PopulateTable(); // Refresh UI
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Viga", $"Foto muutmine ebaõnnestus: {ex.Message}", "OK");
            }
        }

        // New method for sending custom message
        private async void SendCustomMessage(object? sender, EventArgs e)
        {
            string message = messageEntry.Text;

            if (string.IsNullOrWhiteSpace(message))
            {
                await DisplayAlert("Viga", "Palun sisestage sõnum", "OK");
                return;
            }

            if (contacts.Any())
            {
                var randomContact = contacts[random.Next(contacts.Count)];
                bool sendSms = await DisplayAlert("Sõnumi saatmine", $"Saada sõnum \"{message}\" sõbrale {randomContact.Name}?", "SMS", "Email");

                if (sendSms)
                {
                    SendSms(randomContact.Phone);
                }
                else
                {
                    SendEmail(randomContact.Email);
                }
            }
            else
            {
                await DisplayAlert("Viga", "Kontaktide nimekiri on tühi", "OK");
            }
        }

        // Renamed and updated holiday greeting method
        private async void SendRandomHolidayGreeting(object? sender, EventArgs e)
        {
            var holidayGreetings = new List<string>
            {
                "Häid pühi!",
                "Rõõmsaid jõule!",
                "Head uut aastat!",
                "Häid sõbrapäeva!",
                "Häid jaanipäeva!",
                "Häid lihavõttepühi!",
                "Häid iseseisvuspäeva!",
                "Häid vastlapäeva!"
            };

            var randomGreeting = holidayGreetings[random.Next(holidayGreetings.Count)];

            if (contacts.Any())
            {
                var randomContact = contacts[random.Next(contacts.Count)];
                bool sendSms = await DisplayAlert("Pühadetervitus", $"Saata pühadetervitus \"{randomGreeting}\" sõbrale {randomContact.Name}?", "SMS", "Email");

                if (sendSms)
                {
                    // Override the message entry with the random greeting
                    messageEntry.Text = randomGreeting;
                    SendSms(randomContact.Phone);
                }
                else
                {
                    // Override the message entry with the random greeting
                    messageEntry.Text = randomGreeting;
                    SendEmail(randomContact.Email);
                }
            }
            else
            {
                await DisplayAlert("Viga", "Kontaktide nimekiri on tühi", "OK");
            }
        }

        // Toggle contact form visibility
        private void ToggleContactForm(object sender, EventArgs e)
        {
            // Find the contact form in the visual tree (it's the 5th child in our layout)
            if (Content is ScrollView scrollView &&
                scrollView.Content is StackLayout mainLayout &&
                mainLayout.Children.Count >= 5 &&
                mainLayout.Children[4] is VerticalStackLayout contactForm)
            {
                isFormVisible = !isFormVisible;
                contactForm.IsVisible = isFormVisible;

                // Change button text based on form visibility
                if (sender is Button button)
                {
                    button.Text = isFormVisible ? "Peida vorm" : "Lisa uus kontakt";
                }

                // Clear form fields if hiding
                if (!isFormVisible)
                {
                    ClearContactForm();
                }
            }
        }

        // Clear contact form
        private void ClearContactForm()
        {
            nameEntry.Text = string.Empty;
            phoneEntry.Text = string.Empty;
            emailEntry.Text = string.Empty;
            descriptionEntry.Text = string.Empty;
        }

        // Save new contact
        private async void SaveNewContact(object sender, EventArgs e)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(nameEntry.Text) ||
                string.IsNullOrWhiteSpace(phoneEntry.Text) ||
                string.IsNullOrWhiteSpace(emailEntry.Text))
            {
                await DisplayAlert("Viga", "Palun täida kõik kohustuslikud väljad (nimi, telefon, email)", "OK");
                return;
            }

            // Create new contact
            Contact newContact = new Contact
            {
                Name = nameEntry.Text,
                Phone = phoneEntry.Text,
                Email = emailEntry.Text,
                Description = descriptionEntry.Text ?? "",
                Image = "dotnet_bot.png" // Default image
            };

            // Add to contacts list
            contacts.Add(newContact);

            // Refresh the table
            PopulateTable();

            // Clear and hide the form
            ClearContactForm();

            if (Content is ScrollView scrollView &&
                scrollView.Content is StackLayout mainLayout &&
                mainLayout.Children.Count >= 5 &&
                mainLayout.Children[4] is VerticalStackLayout contactForm)
            {
                contactForm.IsVisible = false;
                isFormVisible = false;

                // Update button text
                if (mainLayout.Children[3] is Button addButton)
                {
                    addButton.Text = "Lisa uus kontakt";
                }
            }

            // Show confirmation
            await DisplayAlert("Info", $"Kontakt {newContact.Name} on lisatud!", "OK");
        }
    }

    public class Contact
    { 
        public required string Name { get; set; }
        public required string Phone { get; set; }
        public required string Email { get; set; }
        public required string Description { get; set; }
        public required string Image { get; set; }
    }
}