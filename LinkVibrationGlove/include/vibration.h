#pragma once

#include <functions.h>

class Vibration {
    private:
        int _id;
        int _duration;

    public:
        Vibration(const Commands& commands);
        ~Vibration();

        void set_id(const int& id);
        void set_duration(const int& id);

        const int& get_id() const;
        const int& get_duration() const;
};