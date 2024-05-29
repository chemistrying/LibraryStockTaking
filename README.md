# libraryStockTaking

## Features of the Program:  
* Provides formatting to input to reduce inputting time
* Saves inputs to specific file locations
* Allows deletion of inputs
* Counts the number of books in a file
* Clear guides
* Checks inputs using a booklist exported from the library system  

## How to run the system
1. Pull the repository:
```sh
git clone https://github.com/chemistrying/libraryStockTaking
```
If you want certain development build, you may want to switch to another branch.

2. Install docker and make sure you have docker compose.

3. If you have never used docker compose before, you may want to add permissions first
```sh
# https://stackoverflow.com/questions/68653051/using-docker-compose-without-sudo-doesnt-work
sudo usermod -aG docker $USE
newgrp docker
```
4. Run using docker compose
```sh
docker compose up
```
Or, if you don't want to check logs, you can run in detach mode
```sh
docker compose up -d
```
And shutdown the server with
```sh
docker compose down
```
  
## Future Plans:
* Parse the booklist exported by the library system (Implemented)
* Implement a configuration system (Implemented)
* Use C# to implement the program (Implemented)
* ~~Implement a login system to have a clearer record (Will be implemented in v2)~~ ~~(Substituted by discord API)~~ Implement login system to the backend API
* ~~Add some basic GUI using Win32 API (Wait until main function of the program is implemented)~~ (Cancelled)
* ~~Use .NET to create fancier GUI (Will be implemented later~~ (Cancelled by replacing with webapp <- and implemented <3)
* ~~Put further arguments (e.g: -y) in front of values instead of behind it (Will be implemented later)~~ (Cancelled by no longer using console-like commands)
* Use discord API to create a more manageable stock taking system (Implemented, but with flaws (rate-limited by discord))
* Create a webserver to handle stock taking procedures ~~($\text{Soon}^{\text{TM}}$)~~ (really implemented)
