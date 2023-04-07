#ifndef SerialConnection_h
#define SerialConnection_h

class SerialConnection
{
public:
    SerialConnection(Serial_ &serial, int speed, int timeout);
    int connect() const;
    int available() const;
    int write(const char *payload) const;
    const char *readUntil(char terminator) const;

private:
    Serial_ *m_Serial;
};

#endif