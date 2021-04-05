/* Implements a sequential password cracker for passwords of the exact length of
4 characters using a recursive function*/

#include <stdio.h>
#include <iostream>
#include <chrono>
#include "sha1.h"
#include <cstring>

#define FULLALPHABET 52
#define STARTDIGIT 0

using namespace std;

// char alphabet[26] = {'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k',
// 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'};

unsigned char alphabet[52] = {'a', 'A', 'b', 'B', 'c', 'C', 'd', 'D', 'e', 'E', 'f', 'F',
'g', 'G', 'h', 'H', 'i', 'I', 'j', 'J', 'k', 'K', 'l', 'L', 'm', 'M', 'n', 'N',
'o', 'O', 'p', 'P', 'q', 'Q', 'r', 'R', 's', 'S', 't', 'T', 'u', 'U', 'v', 'V',
'w', 'W', 'x', 'X', 'y', 'Y', 'z', 'Z'};

string stringSecret;
bool found = false;

void printArray(unsigned char* array){
  for (int i = 0; i < strlen((char*)array); i++) cout << array[i];
  cout << endl;
}

bool hashCompare(unsigned char* pword, string secret){
  unsigned char hashPword[20];
  char hexPword[41];
  int arrLength = strlen((char*)pword);
  sha1::calc(pword, arrLength, hashPword);
  sha1::toHexString(hashPword, hexPword);
  string stringPword = hexPword;
  if(stringPword == secret) return true;
  return false;
}

void bruteForceCrack(int startIndex, unsigned char* candidate, int maxLength, int digit){
  unsigned char newCandidate[digit+2];
  newCandidate[digit+1] = '\0';
  for(int c = 0; c < digit+1; c++) newCandidate[c] = candidate[c];
  for(int i = startIndex; i < FULLALPHABET; i++){
    newCandidate[digit] = alphabet[i];
    if(digit < maxLength-1){
      bruteForceCrack(0, newCandidate, maxLength, (digit+1));
    }
    if (found) return;
    if(hashCompare(newCandidate, stringSecret)){
      found = true;
      printArray(newCandidate);
    }
  }
}

int main( int argc, char* argv[]) {

// set up secret to crack
unsigned char secret[4] = {'Z', 'Z', 'Z', 'Z'};
unsigned char hashSecret[20];
char hexSecret[41];
sha1::calc(secret, 4, hashSecret);
sha1::toHexString(hashSecret, hexSecret);
stringSecret = hexSecret;

unsigned char candidate[1];
candidate[0] = '\0';

auto start = chrono::high_resolution_clock::now();
bruteForceCrack(0, candidate, 4, STARTDIGIT);
auto stop = chrono::high_resolution_clock::now();
auto duration = chrono::duration_cast<chrono::milliseconds>(stop - start);
cout << duration.count() << endl;
}
