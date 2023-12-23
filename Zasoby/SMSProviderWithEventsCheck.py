import subprocess
import pyodbc
from datetime import datetime, timedelta
from unidecode import unidecode

def is_user_assigned_to_event(user_id, event_id):
    # Sprawdź, czy użytkownik jest przypisany do wydarzenia
    server = 'localhost,1434'
    database = 'SchoolEventsDB'
    username = 'sa'
    password = 'yourStrongPassword123'
    trust_server_certificate = 'yes'  # lub 'no' w zależności od potrzeb
    conn_str = f'DRIVER={{ODBC Driver 17 for SQL Server}};SERVER={server};DATABASE={database};UID={username};PWD={password};TrustServerCertificate={trust_server_certificate};'

    # Utwórz połączenie z bazą danych
    conn = pyodbc.connect(conn_str)
    cursor = conn.cursor()

    # Zapytanie SQL do sprawdzenia przypisania użytkownika do wydarzenia
    select_query = f"SELECT COUNT(*) FROM EventUser WHERE UserID = {user_id} AND EventID = {event_id}"
    cursor.execute(select_query)
    count = cursor.fetchone()[0]

    # Zamknij połączenie z bazą danych
    cursor.close()
    conn.close()


    return count > 0

def send_sms_and_log_history(user_id, event_id, phone_number, message):
    # Sprawdź, czy wiadomość nie została już wysłana dla danego użytkownika i wydarzenia
    if not is_message_already_sent(user_id, event_id):
        # Komenda ADB do wysłania SMS
        send_sms_command = f'adb shell service call isms 5 i32 0 s16 "com.android.mms.service" s16 "null" s16 {phone_number} s16 "null" s16 "\'{message}\'" s16 "null" s16 "null" i32 0 i64 0'
        subprocess.run(send_sms_command, shell=True)

        # Zapisz historię wysłanej wiadomości do bazy danych
        server = 'localhost,1434'
        database = 'SchoolEventsDB'
        username = 'sa'
        password = 'yourStrongPassword123'
        trust_server_certificate = 'yes'  # lub 'no' w zależności od potrzeb
        conn_str = f'DRIVER={{ODBC Driver 17 for SQL Server}};SERVER={server};DATABASE={database};UID={username};PWD={password};TrustServerCertificate={trust_server_certificate};'

        # Utwórz połączenie z bazą danych
        conn = pyodbc.connect(conn_str)
        cursor = conn.cursor()

        # Zapytanie SQL do sprawdzenia, czy wiadomość już została wysłana
        select_query = f"SELECT COUNT(*) FROM SentMessagesHistory WHERE UserID = {user_id} AND EventID = {event_id}"
        cursor.execute(select_query)
        count = cursor.fetchone()[0]

        if count == 0:
            # Zapytanie SQL do zapisu historii wysłanej wiadomości
            insert_query = f"INSERT INTO SentMessagesHistory (UserID, EventID, PhoneNumber, MessageText, SentDateTime) VALUES ({user_id}, {event_id}, '{phone_number}', '{message}', GETDATE())"
            cursor.execute(insert_query)
            conn.commit()
            print("Wiadomość została wysłana i zapisana do historii.")

        # Zamknij połączenie z bazą danych
        cursor.close()
        conn.close()
    else:
        print("Wiadomość już została wcześniej wysłana dla tego użytkownika i wydarzenia.")

def is_message_already_sent(user_id, event_id):
    # Sprawdź, czy wiadomość już została wysłana
    server = 'localhost,1434'
    database = 'SchoolEventsDB'
    username = 'sa'
    password = 'yourStrongPassword123'
    trust_server_certificate = 'yes'  # lub 'no' w zależności od potrzeb
    conn_str = f'DRIVER={{ODBC Driver 17 for SQL Server}};SERVER={server};DATABASE={database};UID={username};PWD={password};TrustServerCertificate={trust_server_certificate};'

    # Utwórz połączenie z bazą danych
    conn = pyodbc.connect(conn_str)
    cursor = conn.cursor()

    # Zapytanie SQL do sprawdzenia, czy wiadomość już została wysłana
    select_query = f"SELECT COUNT(*) FROM SentMessagesHistory WHERE UserID = {user_id} AND EventID = {event_id}"
    cursor.execute(select_query)
    count = cursor.fetchone()[0]

    # Zamknij połączenie z bazą danych
    cursor.close()
    conn.close()

    return count > 0

def read_users_preferences_from_database():
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
    query = 'SELECT UserID, ReceiveMessages, PreferredDeliveryTime, DeliveryMethod, BlockedHoursStart, BlockedHoursEnd FROM UserPreferences'
    cursor.execute(query)

    # Pobierz wyniki zapytania
    rows = cursor.fetchall()

    # Zamknij połączenie z bazą danych
    cursor.close()
    conn.close()

    # Zwróć listę preferencji użytkowników
    return rows

def get_user_phone_number(user_id):
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

    # Zapytanie SQL do pobrania numeru telefonu użytkownika na podstawie user_id
    query = f'SELECT PhoneNumber FROM Users WHERE UserID = {user_id}'
    cursor.execute(query)

    # Pobierz wynik zapytania
    row = cursor.fetchone()

    # Zamknij połączenie z bazą danych
    cursor.close()
    conn.close()

    # Jeśli użytkownik istnieje, zwróć jego numer telefonu, w przeciwnym razie None
    return row.PhoneNumber if row else None

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

def send_sms(phone_number, message):
    # Komenda ADB do wysłania SMS
    send_sms_command = f'adb shell service call isms 5 i32 0 s16 "com.android.mms.service" s16 "null" s16 {phone_number} s16 "null" s16 "\'{message}\'" s16 "null" s16 "null" i32 0 i64 0'
    subprocess.run(send_sms_command, shell=True)

def schedule_sms_for_events():
    # Pobierz dane z bazy danych dotyczące użytkowników
    user_preferences = read_users_preferences_from_database()

    # Pobierz dane z bazy danych dotyczące wydarzeń
    events = read_events_from_database()

    # Aktualny czas
    current_time = datetime.now()

    # Wysyłaj SMS dla każdego użytkownika z przypomnieniem o nadchodzących wydarzeniach
    for user_pref in user_preferences:
        user_id, receive_messages, preferred_time, delivery_method, blocked_start, blocked_end = user_pref

        # Sprawdź, czy żadne z pól preferencji użytkownika nie jest None (NULL)
        if any(preference is None for preference in [user_id, receive_messages, preferred_time, delivery_method, blocked_start, blocked_end]):
            continue  # Pomiń użytkownika, jeśli jakiekolwiek pole jest NULL

        if receive_messages:  # Sprawdź, czy użytkownik chce otrzymywać wiadomości
            for event in events:
                event_id, title, start_datetime, location = event

                # Przekształć preferred_time na liczbę minut i utwórz obiekt timedelta
                preferred_time_minutes = preferred_time.hour * 60 + preferred_time.minute
                delivery_time = start_datetime - timedelta(minutes=preferred_time_minutes)
                time_difference = start_datetime - current_time
                time_difference = (start_datetime - current_time)
                minutes, seconds = divmod(time_difference.seconds, 60)

                formatted_time_difference = f'{minutes} minut {seconds} sekund'

                if current_time <= delivery_time and is_user_assigned_to_event(user_id, event_id):
                    print(current_time)
                    print(delivery_time)
                    if title is not None and location is not None:
                        title_without_accents = unidecode(title)
                        location_without_accents = unidecode(location)

                        message = f'Przypomnienie o wydarzeniu: {title_without_accents}, {location_without_accents}. Rozpocznie sie za {formatted_time_difference}.'
                    else:
                        message = f'Przypomnienie o wydarzeniu: {title_without_accents}, {location_without_accents}. Rozpocznie sie za {formatted_time_difference}.'
                        print(message)

                        # Pobierz numer telefonu użytkownika
                    user_phone_number = get_user_phone_number(user_id)

                    if user_phone_number:
                        print(user_phone_number)
                        print(message)
                            # Wyślij SMS z użyciem pobranego numeru telefonu

                        send_sms_and_log_history(user_id, event_id, user_phone_number, message)

# Uruchom funkcję do planowania wysyłki SMS dla użytkowników
schedule_sms_for_events()