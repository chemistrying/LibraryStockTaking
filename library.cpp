#include <bits/stdc++.h>

using namespace std;

int main(){
    ifstream fin;
    ofstream fout;
    fin.open("input.txt");
	fout.open("output.txt");
	string c; //format
	cin >> c;
	string s; 
    cin >> s; //fin >> s;

	while(s != "END"){ //!fin.eof() 
		fout << c << s << endl;
        cin >> s; //cin >> s
	}
} 