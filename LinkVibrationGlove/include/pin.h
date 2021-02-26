#pragma once

class Pin {
    private:
        int _pin;
        bool _running;

    public:
        Pin(const int& pin);
        ~Pin();

        void setRunning(const bool running);

        const int& getPin() const;
        const bool& getRunning() const;
};