#include "Engine.h"

Engine::Engine(uint8_t steerLeftPin, uint8_t steerRightPin, uint8_t throttleFwPin, uint8_t throttleBwPin)
{
    m_SteerLeftPin = steerLeftPin;
    m_SteerRightPin = steerRightPin;
    m_ThrottleFwPin = throttleFwPin;
    m_ThrottleBwPin = throttleBwPin;

}

void Engine::init() const
{
    pinMode(m_SteerLeftPin, OUTPUT);
    pinMode(m_SteerRightPin, OUTPUT);
    pinMode(m_ThrottleFwPin, OUTPUT);
    pinMode(m_ThrottleBwPin, OUTPUT);
}

void Engine::steer(int8_t value)
{
    if (value == 0)
    {
        digitalWrite(m_SteerLeftPin, LOW);
        digitalWrite(m_SteerRightPin, LOW);
    }
    else if (value > 0)
    {
        digitalWrite(m_SteerLeftPin, HIGH);
        digitalWrite(m_SteerRightPin, LOW);
    }
    else if (value < 0)
    {
        digitalWrite(m_SteerLeftPin, LOW);
        digitalWrite(m_SteerRightPin, HIGH);
    }

    m_Steer = value;
}

void Engine::throttle(int8_t value)
{
    if (value == 0)
    {
        digitalWrite(m_ThrottleFwPin, LOW);
        digitalWrite(m_ThrottleBwPin, LOW);
    }
    else if (value > 0)
    {
        digitalWrite(m_ThrottleFwPin, HIGH);
        digitalWrite(m_ThrottleBwPin, LOW);
    }
    else if (value < 0)
    {
        digitalWrite(m_ThrottleFwPin, LOW);
        digitalWrite(m_ThrottleBwPin, HIGH);
    }

    m_Throttle = value;
}
