#ifndef Engine_h
#define Engine_h

#include "Arduino.h"

class Engine
{
public:
    Engine(uint8_t steerLeftPin, uint8_t steerRightPin, uint8_t throttleFwPin, uint8_t throttleBwPin);
    void init() const;
    void steer(int8_t value);
    void throttle(int8_t value);


private:
    int8_t m_Steer;
    int8_t m_Throttle;
    int8_t m_SteerLeftPin;
    int8_t m_SteerRightPin;
    int8_t m_ThrottleFwPin;
    int8_t m_ThrottleBwPin;
};

#endif