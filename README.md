

![.NET Build](https://github.com/megjafari/MyPlantPal/actions/workflows/build.yml/badge.svg)


# MyPlantPal 🪴

**Your Personal Plant Care Assistant (Console Application)**

---

##  Overview
MyPlantPal is a console application developed using Spectre.Console to help users track and maintain their houseplants based on species-specific needs and seasonal changes.

<img width="663" height="273" alt="Screenshot 2025-12-01 192153" src="https://github.com/user-attachments/assets/36924477-bb7c-4ab6-8dc5-e5e96ecb6da8" />
<img width="475" height="343" alt="Screenshot 2025-12-01 192447" src="https://github.com/user-attachments/assets/78a762f2-f87d-4ef4-ad54-016ed2fc9a70" />


##  Features

* **User Authentication:** Secure registration and login (passwords hashed using SHA256).
* **Seasonal Logic:** Dynamically calculates watering intervals based on the current month (e.g., Winter requires less frequent watering).
* **Plant Catalog:** Access to a catalog of 50 popular indoor plant species.
* **Watering Reminders:** Displays overdue plants prominently in the main menu.
* **Statistics:** Provides user performance metrics, including successful watering history.
* **Data Persistence:** All user and plant data is saved locally.
* **Account Management:** Options to change passwords and delete the account (with cascading data removal).

##  Architecture and Data

The project utilizes a **Service Layer** with Dependency Injection to separate business logic from the user interface.

* **Data Source:** All core data (user accounts, plant instances, and the plant catalog) is stored locally in **JSON files**.
* **Time Tracking:** **UTC time** is used consistently across all time-based tracking logic to prevent timezone-related errors.
* **Security:** Passwords are securely hashed.

##  Project layout

<img width="504" height="554" alt="Screenshot 2025-12-01 192101" src="https://github.com/user-attachments/assets/70ca9764-be2e-46ca-9ad5-ca9890b22eb2" />



##  Installation & Running

**To run the application, you will need:**

1.  .NET Runtime (Version X.X, depending on your project's target framework).

### Option 1: Download the Executable (Recommended for End-Users)

1.  Go to the **Releases** tab on this GitHub repository.
2.  Download the latest ZIP file (e.g., `MyPlantPal_v1.0.0.zip`).
3.  Extract the contents and run the executable file (`MyPlantPal.exe`).

### Option 2: Build from Source

1.  Clone this repository: `git clone [Your GitHub URL]`
2.  Navigate to the project's root directory.
3.  Execute the application using the command `dotnet run`.
