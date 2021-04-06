/* Implements a sequential password cracker for passwords of the exact length of
4 characters using a recursive function*/

#include <stdio.h>
#include <iostream>
#include <chrono>
#include "sha1.h"
#include <cstring>
#include <omp.h>
#include <vector>

#define FULLALPHABET 52
#define T_NUM 6
#define STARTDIGIT 0
#define MAXLENGTH 4

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

void bruteForceCrack(int startIndex, int endIndex, unsigned char* candidate, int maxLength, int digit){
  unsigned char newCandidate[digit+2];
  newCandidate[digit+1] = '\0';
  for(int c = 0; c < digit+1; c++) newCandidate[c] = candidate[c];
  // let each call only handle one first letter
  // int loopStop = FULLALPHABET;
  // if(digit == 0) loopStop = startIndex;
  // for(int i = startIndex; i <= loopStop; i++){
  for(int i = startIndex; i < endIndex; i++){
    newCandidate[digit] = alphabet[i];
    if(digit < maxLength-1){
      bruteForceCrack(0, FULLALPHABET, newCandidate, maxLength, (digit+1));
    }
    if (found) return;
    if(hashCompare(newCandidate, stringSecret)){
      found = true;
      printArray(newCandidate);
    }
  }
}

long parallel() {
	// set up secret to crack
    unsigned char secret[4] = {'Z', 'Z', 'Z', 'Z'};
    unsigned char hashSecret[20];
    char hexSecret[41];
    sha1::calc(secret, 4, hashSecret);
    sha1::toHexString(hashSecret, hexSecret);
    stringSecret = hexSecret;
    found = false;

    unsigned char candidate[1];
    candidate[0] = '\0';

    auto start = chrono::high_resolution_clock::now();
    #pragma omp parallel for num_threads(T_NUM)

    // --- every letter gets its own thread ---
    // for(int i = 0; i < FULLALPHABET; i++) {
    //   bruteForceCrack(i, (i+1), candidate, MAXLENGTH, STARTDIGIT);
    // }

    // --- start chunking from the end of the alphabet ---
    for(int i = FULLALPHABET; i > 0; i -= FULLALPHABET / T_NUM){
      //cout << i << endl;
      int startIndex = i - FULLALPHABET / T_NUM;
      if (startIndex < 0) startIndex = 0;
      bruteForceCrack(startIndex, i, candidate, MAXLENGTH, STARTDIGIT);
    }

    // --- start chunking from start of the alphabet ---
    // for(int i = 0; i < FULLALPHABET; i += FULLALPHABET / T_NUM) {
    //   cout << i << endl;
    //   int endIndex = i + FULLALPHABET / T_NUM;
    //   if(endIndex > FULLALPHABET) endIndex = FULLALPHABET;
    //   bruteForceCrack(i, endIndex, candidate, MAXLENGTH, STARTDIGIT);
    // }

    auto stop = chrono::high_resolution_clock::now();
    auto duration = chrono::duration_cast<chrono::milliseconds>(stop - start);
    cout << duration.count() << endl;

    return duration.count();
}

void benchmark() {
    cout << "----- STARTING BENCHMARK -----" << endl;
    cout << "Initializing..." << endl;
    for (int i = 0; i < BENCHMARK_DISCARD_AMOUNT; i++) { // Discard => warmup
        parallel();
    }
    cout << "START" << endl;
    long duration = 0;
    for (int i = 0; i < BENCHMARK_MAX_ITERATIONS; i++) {
        duration += parallel();
    }
    long average = duration / BENCHMARK_MAX_ITERATIONS;

    cout << "FINISHED! Average:" << endl;
    cout << average << endl;
}

int main( int argc, char* argv[]) {
	benchmark();

    system("pause");
}
