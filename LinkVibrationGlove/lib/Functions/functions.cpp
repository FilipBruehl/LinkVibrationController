#include "functions.h"

Commands split(const std::string& string) {
  Commands strings;
  std::istringstream f(string);
  std::string s;
  while(std::getline(f, s, ' ')) {
    strings.push_back(s);
  }
  return strings;
};

int convert_string_int(const std::string& string) {
    std::stringstream s(string);
    int value;
    s >> value;
    return value;
};