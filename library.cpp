#include "library.h"

int main(){
	if (preload){
		cout << "Loading Booklist...\n";
		booklist = loadBooklist(defaultFileLocation);
	}
	systemMessage();
  	getline(cin, commandInput); //fin >> s;
	while(commandInput != "END"){ //!fin.eof() 
		if (commandInput[0] == ';'){
			formatChange();
		}else if (commandInput[0] == '/'){//command
			commandHandling();
		}else if (int(buffer.size()) >= limit){
			cout << "Buffer reached maximum. Please save before entering more data." << endl;
		}else{
			buffer.push_back(format + commandInput + format_back);
		}
		getline(cin, commandInput);
	}
}

//current problems
//error when deleting last input