#include <bits/stdc++.h>

using namespace std;

int main(){
	string format;
	string tracking = "";
	string buffer[50];
	int limit = 0;
	ofstream fout;

	string s; 
    getline(cin, s); //fin >> s;
	while(s != "END"){ //!fin.eof() 
		if (s[0] == '['){
			format = s.substr(1, s.size() - 2);
		}else if (s[0] == '/'){//command
			string sa = s.substr(1, 4);
			if (sa == "SAVE"){
				string filename = s.substr(6, s.size() - 5);
				if (filename != "") tracking = filename;
				fout.open(tracking + ".txt");
				for (int i = 0; i < limit; i++){
					fout << buffer[i] << endl;
				}
				fout.flush();
				fout.close();
			}
		}else if (limit == 50){
			cout << "Buffer reached maximum. Please save before entering mroe data." << endl;
		}else{
			buffer[limit++] = format + s;
		}
		cin >> s;
	}
}