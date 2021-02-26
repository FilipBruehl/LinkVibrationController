#pragma once

#include <sstream>
#include <iostream>

#include <vector>
#include <string>

typedef std::vector<std::string> Commands;

Commands split(const std::string& string);
int convert_string_int(const std::string& string);
