#ifndef SerialConnection_h
#define SerialConnection_h

#include "Arduino.h"

class SerialConnection
{
public:
    SerialConnection(HardwareSerial &serial, uint32_t speed, uint32_t timeout);
    void init() const;
    bool connect() const;
    bool dataAvailable() const;
    bool write(String payload) const;
    String readUntil(char terminator) const;

private:
    uint32_t m_Speed;
    uint32_t m_Timeout;
    HardwareSerial *m_Serial;
};

#endif