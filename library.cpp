#include <bits/stdc++.h>

using namespace std;

string format;
string tracking = "PLEASE_TYPE_A_FILENAME_BEFORE_YOUR_FIRST_SAVE";
vector<string> buffer;
int limit = 50;
int bufferIterator = 0;
ofstream fout;
string s;

bool cmdError = 0;

void commandSyntaxError(){
	cout << "Syntax Error: Pleas retype the command again." << endl;
	cmdError = 0;
}

void save(){
	if (s.size() > 6){
		string filename = s.substr(6, s.size() - 5);
		tracking = filename;
	}else{
		cmdError = 1;
	}
	fout.open(tracking + ".txt", ios_base::app);
	for (int i = 0; i < buffer.size(); i++){
		fout << buffer[i] << endl;
	}
	buffer.clear();
	fout.flush();
	fout.close();
}

void del(){
	if (s.size() < 6){
		buffer.pop_back();
	}else{
		string sub = s.substr(5, s.size() - 5);
		int subs = sub.size();
		for (int i = 0; i < subs; i++) if(!((sub[i] >= '0' && sub[i] <= '9') || sub[i] == '-')) cmdError = 1;

		if (!cmdError){
			int pos = stoi(sub);
			if (pos < 0){
				buffer.erase(buffer.end() + pos - 1);
				bufferIterator--;
			}else{
				buffer.erase(buffer.begin() + pos);
			}
		}
	}
}

void changeLimit(){
	if (s.size() < 8) cmdError = 1;//8
	else{
		string sub = s.substr(7, s.size() - 7);
		int subs = sub.size();
		for (int i = 0; i < subs; i++) if (!(sub[i] >= '0' && sub[i] <= '9')) cmdError = 1;

		if (!cmdError) limit = stoi(sub);
	}
}

string toUpper(string str){
	int ss = s.size();
	for (int i = 0; i < ss; i++) if (str[i] >= 'a' && str[i] <= 'z') str[i] -= 32;
	return str;
}

void commandHandling(){
	string cmd;
	//save
	cmd = toUpper(s.substr(1, 4));
	if (cmd == "SAVE") save();

	//undo
	if (cmd == "UNDO");

	//delete
	cmd = toUpper(s.substr(1, 3));
	if (cmd == "DEL") del();

	//limit
	cmd = toUpper(s.substr(1, 5));
	if (cmd == "LIMIT") changeLimit();

	if (cmdError){
		commandSyntaxError();
	}
}

int main(){
    getline(cin, s); //fin >> s;
	while(s != "END"){ //!fin.eof() 
		if (s[0] == '['){
			format = s.substr(1, s.size() - 2);
		}else if (s[0] == '/'){//command
			commandHandling();
		}else if (bufferIterator >= limit){
			cout << "Buffer reached maximum. Please save before entering more data." << endl;
		}else{
			buffer.push_back(format + s);
			bufferIterator++;
		}
		getline(cin, s);
	}
}