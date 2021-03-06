/* Implements a sequential password cracker for passwords of the exact length of
4 characters using a recursive function*/

#include <stdio.h>
#include <iostream>
#include <chrono>
#include "sha1.h"
#include <cstring>

#define FULLALPHABET 52
#define STARTDIGIT 0

// Benchmark
#define BENCHMARK_MAX_ITERATIONS 10
#define BENCHMARK_DISCARD_AMOUNT 3

using namespace std;

// char alphabet[26] = {'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k',
// 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'};

unsigned char alphabet[52] = {'a', 'A', 'b', 'B', 'c', 'C', 'd', 'D', 'e', 'E', 'f', 'F',
'g', 'G', 'h', 'H', 'i', 'I', 'j', 'J', 'k', 'K', 'l', 'L', 'm', 'M', 'n', 'N',
'o', 'O', 'p', 'P', 'q', 'Q', 'r', 'R', 's', 'S', 't', 'T', 'u', 'U', 'v', 'V',
'w', 'W', 'x', 'X', 'y', 'Y', 'z', 'Z'};

unsigned char hashSecret[20];
bool found = false;

void printArray(unsigned char* array){
  for (int i = 0; i < strlen((char*)array); i++) cout << array[i];
  cout << endl;
}

bool hashCompare(unsigned char* pword, unsigned char *secret){
  unsigned char hashPword[20];
  int arrLength = strlen((char*)pword);
  sha1::calc(pword, arrLength, hashPword);
  return memcmp(hashPword, hashSecret, 20) == 0;
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
    if(hashCompare(newCandidate, hashSecret)){
      found = true;
      printArray(newCandidate);
    }
  }
}

long serial() {
    // set up secret to crack
    unsigned char secret[4] = { 'Z', 'Z', 'Z', 'Z' };
    char hexSecret[41];
    sha1::calc(secret, 4, hashSecret);
    found = false;

    unsigned char candidate[1];
    candidate[0] = '\0';

    auto start = chrono::high_resolution_clock::now();
    bruteForceCrack(0, candidate, 4, STARTDIGIT);
    auto stop = chrono::high_resolution_clock::now();
    auto duration = chrono::duration_cast<chrono::milliseconds>(stop - start);
    cout << duration.count() << endl;

    return duration.count();
}

void benchmark() {
    cout << "----- STARTING BENCHMARK -----" << endl;
    cout << "Initializing..." << endl;
    for (int i = 0; i < BENCHMARK_DISCARD_AMOUNT; i++) { // Discard => warmup
        serial();
    }
    cout << "--- START ---" << endl;
    long duration = 0;
    for (int i = 0; i < BENCHMARK_MAX_ITERATIONS; i++) {
        duration += serial();
    }
    long average = duration / BENCHMARK_MAX_ITERATIONS;

    cout << "FINISHED! Average:" << endl;
    cout << average << endl;
}

int main( int argc, char* argv[]) {
    benchmark();

    system("pause");
}
