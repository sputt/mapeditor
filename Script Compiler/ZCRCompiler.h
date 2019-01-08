
#pragma once
using namespace System;
#include <msclr\marshal_cppstd.h>

#pragma managed(push, off)
#include <string>
void read_commands(std::string filename);
void read_code(const char *filename);
std::string run_compile(const std::string& name, const std::string& filename);
#pragma managed(pop)

namespace CScriptCompiler {

	__declspec(dllexport) public ref class ZCRCompiler {
	public:
		ZCRCompiler(String ^zeldaPath) {
			std::string engine_path = msclr::interop::marshal_as<std::string>(zeldaPath + "\\scriptengine-new.asm");
			read_commands(engine_path.c_str());
		}

		String ^Compile(String ^name, String ^input) {
			auto RandomFile = IO::Path::GetTempFileName() + ".zcr";

			auto Stream = gcnew IO::StreamWriter(RandomFile);
			Stream->Write(input);
			Stream->Close();

			std::string input_file = msclr::interop::marshal_as<std::string>(RandomFile);
			read_code(input_file.c_str());

			auto OutputFilePath = IO::Path::GetTempFileName() + ".zcr";
			std::string output_file = msclr::interop::marshal_as<std::string>(OutputFilePath);

			std::string script_name = msclr::interop::marshal_as<std::string>(name);
			return gcnew System::String(run_compile(script_name, output_file).c_str());
		}
	};

}
