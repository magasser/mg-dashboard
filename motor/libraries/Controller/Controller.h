#ifndef Controller_h
#define Controller_h

#include "Arduino.h"
#include "Engine.h"

class Controller
{
public:
    Controller(Engine &engine);
    void init() const;
    bool execute(String *parameters, uint8_t parameterCount);


private:
    Engine *m_Engine;
};

#endif