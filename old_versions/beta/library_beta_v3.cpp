#include <bits/stdc++.h>

using namespace std;

string format;
string format_back;
string tracking = "PLEASE_TYPE_A_FILENAME_BEFORE_YOUR_FIRST_SAVE";
vector<string> buffer;
int limit = 50;
int bufferIterator = 0;
ifstream fin;
ofstream fout;
string s;
stack<pair<string, int>> undoStack;

bool cmdError = 0;

void commandSyntaxError(){
	cout << "Syntax Error: Pleas retype the command again." << endl;
	cmdError = 0;
}

string ordinal(int n){
	if (n % 100 >= 11 && n % 100 <= 13) return "th";
	else if (n % 10 == 1) return "st";
	else if (n % 10 == 2) return "nd";
	else if (n % 10 == 3) return "rd";
	else return "th";
}

string toUpper(string str){
	int ss = s.size();
	for (int i = 0; i < ss; i++) if (str[i] >= 'a' && str[i] <= 'z') str[i] -= 32;
	return str;
}

void save(){
	if (s.size() > 6){
		string filename = s.substr(6, s.size() - 5);
		tracking = filename;
	}
	fout.open(tracking + ".txt", ios_base::app);
	for (int i = 0; i < buffer.size(); i++){
		fout << buffer[i] << endl;
	}
	buffer.clear();
	fout.flush();
	fout.close();

	cout << "Saved to " << tracking << ".txt." << endl;
}


void undo(){
	
}

void del(){
	if (s.size() < 6){
		buffer.pop_back();
		bufferIterator--;
		cout << "Deleted last input." << endl;
	}else{
		string sub = s.substr(5, s.size() - 5);
		int subs = sub.size();
		for (int i = 0; i < subs; i++) if(!((sub[i] >= '0' && sub[i] <= '9') || sub[i] == '-')) cmdError = 1;

		if (!cmdError){
			int pos = stoi(sub);
			if (pos < 0){
				int ord = buffer.size() + pos + 1; 
				buffer.erase(buffer.end() + pos);
				bufferIterator--;

				cout << "Deleted " << ord << ordinal(ord) << " input." << endl;

			}else{
				int ord = pos + 1;
				buffer.erase(buffer.begin() + pos);
				bufferIterator--;

				cout << "Deleted " << ord << ordinal(ord) << " input." << endl;
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

		if (!cmdError){
			limit = stoi(sub);
			cout << "Changed limit to " << limit << "." << endl;
		}
	}
}

void clear(){
	cout << "Are you sure to clear the undo stack? Type \"Y\" to confirm. ";
	char c;
	cin >> c;
	if (c == 'Y'){
		int sts = undoStack.size() - 1;
		while (sts-- >= 0){
			undoStack.pop();
		}
		cout << "Undo stack cleared." << endl;
	}else{
		cout << "Clear command failed to execute." << endl;
	}
}

void count(){
  	if (s.size() < 8) cmdError = 1;
  	else{
    	string sub = s.substr(7, s.size() - 7);
    	int subs = sub.size();
		fin.open(subs + ".txt");
		int cnt = 0;
		while (!fin.eof()){
			string _;
			getline(fin, _);
			cnt++;
		}
		cout << cnt << endl;
		fin.close();
  	}
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

	//clear
	if (cmd == "CLEAR") clear();

  	if (cmd == "COUNT") count();

	if (cmdError){
		commandSyntaxError();
	}
}

void formatChange(){
	int pos = s.find('*');
	if (pos != -1){
		format = s.substr(1, pos - 1);
		format_back = s.substr(pos + 1, s.size() - pos - 1);
	}else{
		format = s.substr(1, s.size() - 1);
		format_back = "";
	}
}

int main(){
    getline(cin, s); //fin >> s;
	while(s != "END"){ //!fin.eof() 
		if (s[0] == ';'){
			formatChange();
		}else if (s[0] == '/'){//command
			commandHandling();
		}else if (bufferIterator >= limit){
			cout << "Buffer reached maximum. Please save before entering more data." << endl;
		}else{
			buffer.push_back(format + s + format_back);
			bufferIterator++;
		}
		getline(cin, s);
	}
}