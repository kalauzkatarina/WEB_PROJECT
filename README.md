#**WEB_PROJECT – Travel Agency Web Application**

This project is developed as part of the Web Programming – Applied Software Engineering (PSI 2024/2025) course.
It represents a simulation of an online travel agency system that allows users to browse travel arrangements, make reservations, manage accommodations, and interact based on their assigned user roles.

##**Features**
  **Unauthenticated Users**
  ● Browse all upcoming travel arrangements
  ● Search and filter by:
    ◦ Start/end dates
    ◦ Transport type
    ◦ Arrangement type
    ◦ Name
  ● Sort results by name or date
  ● View detailed arrangement information, including accommodations and accommodation units
  ● View comments for accommodations
  ● Register and log in

  **Tourist**
  ● Create reservations (select available accommodation unit)
  ● Cancel reservations (only before the trip starts)
  ● View all personal reservations (past, upcoming, active, cancelled)
  ● Search and sort personal reservation history
  ● View detailed reservation information
  ● Leave comments on accommodations after returning from a trip
  ● Edit personal profile

  **Manager**
  ● Create, update, view, and logically delete their own travel arrangements
  ● Create and manage accommodations and accommodation units
  ● View all reservations related to their arrangements
  ● Approve or reject tourist comments (only for their arrangements)
  ● View all comments related to their arrangements
  ● Search and sort arrangements, accommodations, and units

  **Administrator**
  ● View all system users
  ● Search users by name and surname
  ● Filter by role
  ● Register new managers
  ● Cannot be created dynamically — loaded from file

  **Entities**
  ● User (Admin / Manager / Tourist)
  ● Arrangement
  ● Accommodation
  ● Accommodation Unit
  ● Reservation
  ● Comment

All required attributes defined by the project specification are implemented.

##**Data Storage**
All application data is permanently stored in **text-based files**:

Supported formats: JSON, XML, CSV, TSV, or any DSV format.
*(The chosen format is JSON.)*

Test data is included to demonstrate all functionalities.

##**Technologies Used**
Only the technologies allowed by the course are used.
Typical stack for this project includes:
  ● **HTML5**
  ● **CSS3**
  ● **Bootstrap5**
  ● **JavaScript**

##**How to Run the Application**
1. Clone the repository:
  *git clone https://github.com/yourusername/WEB_PROJECT*

2. Open the project in any local development server
  Example: VS Code Live Server, WAMP, XAMPP, or any simple HTTP server

3. Navigate to:
  *http://localhost:<port>/index.html*

4. Use the application through the browser interface

##**Course**
This project was created for the Web Programming (2024/2025) course at the
Faculty of Technical Sciences, University of Novi Sad.
