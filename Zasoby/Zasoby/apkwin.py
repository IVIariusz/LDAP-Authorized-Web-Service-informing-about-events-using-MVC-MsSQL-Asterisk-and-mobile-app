from kivy.app import App
from kivy.uix.boxlayout import BoxLayout
from kivy.uix.label import Label
from kivy.uix.textinput import TextInput
from kivy.uix.button import Button
from kivy.uix.screenmanager import ScreenManager, Screen
from kivy.uix.scrollview import ScrollView
from kivy.core.window import Window

import pyodbc

def read_events_from_database():
    # Ustawienia dostępu do bazy danych MSSQL
    server = 'localhost,1434'
    database = 'SchoolEventsDB'
    username = 'sa'
    password = 'yourStrongPassword123'
    trust_server_certificate = 'yes'  # lub 'no' w zależności od potrzeb
    conn_str = f'DRIVER={{ODBC Driver 17 for SQL Server}};SERVER={server};DATABASE={database};UID={username};PWD={password};TrustServerCertificate={trust_server_certificate};'
    
    # Utwórz połączenie z bazą danych
    conn = pyodbc.connect(conn_str)
    cursor = conn.cursor()

    # Tutaj umieść zapytanie SQL do pobrania danych z bazy
    query = 'SELECT EventID, Title, StartDateTime, Location FROM Events'
    cursor.execute(query)

    # Pobierz wyniki zapytania
    rows = cursor.fetchall()

    # Zamknij połączenie z bazą danych
    cursor.close()
    conn.close()

    # Zwróć listę wydarzeń
    return rows

class LoginScreen(BoxLayout):

    def __init__(self, **kwargs):
        super(LoginScreen, self).__init__(**kwargs)
        self.orientation = 'vertical'
        
        print("Entering Login Screen")

        self.add_widget(Label(text='Login', size_hint_y=None, height=30))
        self.username = TextInput(multiline=False, size_hint_y=None, height=30)
        self.add_widget(self.username)

        self.add_widget(Label(text='Password', size_hint_y=None, height=30))
        self.password = TextInput(password=True, multiline=False, size_hint_y=None, height=30)
        self.add_widget(self.password)

        self.login_button = Button(text='Login', size_hint_y=None, height=30)
        self.login_button.bind(on_press=self.on_login)  
        self.add_widget(self.login_button)

        self.message = Label(text='', size_hint_y=None, height=30)
        self.add_widget(self.message)

    def on_login(self, instance):
        # Get the entered login
        login = self.username.text
        # Get the instance of the EventsScreen
        events_screen = app.root.get_screen('events').children[0]
        # Call the update_login method on the EventsScreen instance
        events_screen.update_login(login)
        # Move to the next screen
        app.root.current = 'events'

    def check_credentials(self, instance):
        username = self.username.text
        password = self.password.text

        try:
            server = 'localhost,1434'
            database = 'SchoolEventsDB'
            db_username = 'sa'
            db_password = 'yourStrongPassword123'
            trust_server_certificate = 'yes'
            conn_str = f'DRIVER={{ODBC Driver 17 for SQL Server}};SERVER={server};DATABASE={database};UID={db_username};PWD={db_password};TrustServerCertificate={trust_server_certificate};'

            conn = pyodbc.connect(conn_str)
            cursor = conn.cursor()

            cursor.execute("SELECT * FROM Users WHERE Email = ? AND PasswordHash = ?", (username, password))
            user = cursor.fetchone()

            if user:
                self.message.text = "Login successful"
                app.root.current = 'main'
            else:
                self.message.text = "Login failed"

            cursor.close()
            conn.close()

        except Exception as e:
            self.message.text = f"Error: {str(e)}"


class EventsScreen(BoxLayout):
    def __init__(self, login, **kwargs):
        super(EventsScreen, self).__init__(**kwargs)
        self.orientation = 'vertical'

        print(f"Entering Events Screen for user: {login}")

        # Fetch user_id from the database using login
        self.user_id = self.get_user_id_from_login(login)

        # Create a ScrollView
        scroll_view = ScrollView(size_hint=(1, None), size=(Window.width, Window.height - 100))
        self.add_widget(scroll_view)

        # Create a layout for the buttons inside the ScrollView
        self.layout = BoxLayout(orientation='vertical', size_hint_y=None, height=0)
        scroll_view.add_widget(self.layout)

        self.add_widget(Label(text='Events', size_hint_y=None, height=30))

    def update_login(self, login):
        self.login = login
        print(f"Entering Events Screen for user: {login}")
        # Fetch user_id from the database using login
        self.user_id = self.get_user_id_from_login(login)
        # Update the events based on the new login
        self.update_events()

    def update_events(self):
        events = self.read_events_for_user()
        self.layout.clear_widgets()  # Clear existing widgets in the layout
        # Create buttons for each event
        for event in events:
            button = Button(
                text=f'{event[1]}\n{event[2]} at {event[3]}',
                size_hint_y=None,
                height=100,
                on_press=self.on_event_button_press
            )
            self.layout.add_widget(button)

        # Fetch user_id from the database using login
        self.user_id = self.get_user_id_from_login(self.login)

        self.add_widget(Label(text='Events', size_hint_y=None, height=30))

        events = self.read_events_for_user()

        # Create a ScrollView
        scroll_view = ScrollView(size_hint=(1, None), size=(Window.width, Window.height - 100))
        self.add_widget(scroll_view)

        # Create a layout for the buttons inside the ScrollView
        layout = BoxLayout(orientation='vertical', size_hint_y=None, height=len(events) * 100)
        scroll_view.add_widget(layout)

        # Create buttons for each event
        for event in events:
            button = Button(
                text=f'{event[1]}\n{event[2]} at {event[3]}',
                size_hint_y=None,
                height=100,
                on_press=self.on_event_button_press
            )
            layout.add_widget(button)

        # Create an "Exit" button
        exit_button = Button(text='Exit', size_hint_y=None, height=100, on_press=self.exit_events)
        self.add_widget(exit_button)

    def get_user_id_from_login(self, login):
        # Function to retrieve user_id from the database using login
        server = 'localhost,1434'
        database = 'SchoolEventsDB'
        username = 'sa'
        password = 'yourStrongPassword123'
        trust_server_certificate = 'yes'  # or 'no' depending on your needs
        conn_str = f'DRIVER={{ODBC Driver 17 for SQL Server}};SERVER={server};DATABASE={database};UID={username};PWD={password};TrustServerCertificate={trust_server_certificate};'

        # Create a connection to the database
        conn = pyodbc.connect(conn_str)
        cursor = conn.cursor()

        select_query = f"SELECT UserID FROM Users WHERE Email = '{login}'" 
        cursor.execute(select_query)

        # Fetch the result
        result = cursor.fetchone()

        if result:
            user_id = result[0]
        else:
            user_id = None

        # Close the database connection
        cursor.close()
        conn.close()

        return user_id

    def read_events_for_user(self):
        # Function to retrieve events assigned to the user from the database
        if self.user_id is not None:
            server = 'localhost,1434'
            database = 'SchoolEventsDB'
            username = 'sa'
            password = 'yourStrongPassword123'
            trust_server_certificate = 'yes'  # or 'no' depending on your needs
            conn_str = f'DRIVER={{ODBC Driver 17 for SQL Server}};SERVER={server};DATABASE={database};UID={username};PWD={password};TrustServerCertificate={trust_server_certificate};'

            # Create a connection to the database
            conn = pyodbc.connect(conn_str)
            cursor = conn.cursor()

            # Query to retrieve events assigned to the user
            select_query = f"""
                SELECT E.EventID, E.Title, E.StartDateTime, E.Location
                FROM Events E
                JOIN EventUser EU ON E.EventID = EU.EventID
                WHERE EU.UserID = {self.user_id}
            """
            cursor.execute(select_query)
            events = cursor.fetchall()

            # Close the database connection
            cursor.close()
            conn.close()

            return events
        else:
            return []

    def on_event_button_press(self, instance):
        print(f"Button pressed: {instance.text}")

    def exit_events(self, instance):
        app.root.current = 'main'


class MainScreen(BoxLayout):

    def __init__(self, **kwargs):
        super(MainScreen, self).__init__(**kwargs)
        self.orientation = 'horizontal'  # Use horizontal layout
        
        print("Entering Main Screen")

        self.events_button = Button(text='Wydarzenia', size_hint_y=None, height=80)
        self.events_button.bind(on_press=self.show_events)
        self.add_widget(self.events_button)

        self.preferences_button = Button(text='Preferencje', size_hint_y=None, height=80)
        self.preferences_button.bind(on_press=self.show_preferences)
        self.add_widget(self.preferences_button)

        self.logout_button = Button(text='Wyloguj', size_hint_y=None, height=80)
        self.logout_button.bind(on_press=self.logout)
        self.add_widget(self.logout_button)

    def show_events(self, instance):
        print("Switching to Events Screen")
        app.root.current = 'events'

    def show_preferences(self, instance):
        print("Showing preferences")

    def logout(self, instance):
        print("Logging out")
        app.root.current = 'login'


class EventsApp(App):

    def build(self):
        print("Building the app")
        sm = ScreenManager()

        login_screen = Screen(name='login')
        login_screen.add_widget(LoginScreen())
        sm.add_widget(login_screen)

        main_screen = Screen(name='main')
        main_screen.add_widget(MainScreen())
        sm.add_widget(main_screen)


        events_screen_instance = Screen(name='events')
        events_screen_instance.add_widget(EventsScreen(login='')) 
        sm.add_widget(events_screen_instance)

        return sm

if __name__ == '__main__':
    app = EventsApp()
    app.run()
