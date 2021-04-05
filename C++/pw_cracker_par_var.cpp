/* Implements a parallel password cracker for passwords of variable length up to
4 characters */

#include <stdio.h>
#include <iostream>
#include <chrono>
#include "sha1.h"
#include <omp.h>
#include <cstring>

#define FULLALPHABET 52
using namespace std;

void printArray(unsigned char* array){
  for (int i = 0; i < strlen((char*)array); i++) cout << array[i];
  cout << endl;
}

bool hashCompare(unsigned char* pword, string secret){
  unsigned char hash_pword[20];
  char hex_pword[41];
  int arrLength = strlen((char*)pword);
  sha1::calc(pword, arrLength, hash_pword);
  sha1::toHexString(hash_pword, hex_pword);
  string string_pword = hex_pword;
  if(string_pword == secret) return true;
  return false;
}

void signalSuccess(unsigned char* res, auto start){
  auto stop = chrono::high_resolution_clock::now();
  auto duration = chrono::duration_cast<chrono::milliseconds>(stop - start);
  cout << duration.count() << endl;
  printArray(res);
}

int main( int argc, char* argv[]) {

// char alphabet[26] = {'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k',
// 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'};

unsigned char alphabet[52] = {'a', 'A', 'b', 'B', 'c', 'C', 'd', 'D', 'e', 'E', 'f', 'F',
'g', 'G', 'h', 'H', 'i', 'I', 'j', 'J', 'k', 'K', 'l', 'L', 'm', 'M', 'n', 'N',
'o', 'O', 'p', 'P', 'q', 'Q', 'r', 'R', 's', 'S', 't', 'T', 'u', 'U', 'v', 'V',
'w', 'W', 'x', 'X', 'y', 'Y', 'z', 'Z'};

// set up secret to crack
unsigned char secret[4] = {'Z', 'Z', 'Z', 'Z'};
unsigned char hashSecret[20];
char hexSecret[41];
sha1::calc(secret, 4, hashSecret);
sha1::toHexString(hashSecret, hexSecret);
string stringSecret = hexSecret;

auto start = chrono::high_resolution_clock::now();
#pragma omp parallel for num_threads(6)
for(int i = 0; i < FULLALPHABET; i++){
  unsigned char c1[2] = {alphabet[i], '\0'};
  if(hashCompare(c1, stringSecret)) signalSuccess(c1, start);
  for(int i = 0; i < FULLALPHABET; i++){
    unsigned char c2[3] = {c1[0], alphabet[i], '\0'};
    if(hashCompare(c2, stringSecret)) signalSuccess(c2, start);
    for(int i = 0; i < FULLALPHABET; i++){
      unsigned char c3[4] = {c2[0], c2[1], alphabet[i], '\0'};
      if(hashCompare(c3, stringSecret)) signalSuccess(c3, start);
      for(int i = 0; i < FULLALPHABET; i++){
        unsigned char c4[5] = {c3[0], c3[1], c3[2], alphabet[i], '\0'};
        if(hashCompare(c4, stringSecret)) signalSuccess(c4, start);
      }
    }
  }
}

return 0;

}
